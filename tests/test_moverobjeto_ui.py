"""Pruebas de UI y lógica contra MoverObjeto.exe usando pytest y pywinauto.

Las pruebas están pensadas para ejecutarse en Windows porque dependen de la
automatización UIA. Si el ejecutable no está compilado en el directorio
esperado, las pruebas se omitirán con un mensaje claro.
"""

from __future__ import annotations

import platform
import time
from pathlib import Path

import pytest

pywinauto = pytest.importorskip("pywinauto", reason="pywinauto es requerido para las pruebas de UI")
from pywinauto.application import Application
from pywinauto.controls.uia_controls import ComboBoxWrapper
from pywinauto.keyboard import send_keys


WINDOWS_ONLY = pytest.mark.skipif(platform.system() != "Windows", reason="Las pruebas de UI solo se ejecutan en Windows")
EXE_PATH = Path(__file__).resolve().parents[1] / "MoverObjeto" / "bin" / "Debug" / "net9.0-windows" / "MoverObjeto.exe"
STARTUP_TIMEOUT = 10
GAME_READY_TIMEOUT = 20


def _ensure_executable() -> None:
    if not EXE_PATH.exists():
        pytest.skip(f"MoverObjeto.exe no está disponible en {EXE_PATH}")


def _launch_selection_app() -> tuple[Application, object]:
    _ensure_executable()
    app = Application(backend="uia").start(str(EXE_PATH))
    window = app.window(title_re="Seleccionar jugador")
    window.wait("visible", timeout=STARTUP_TIMEOUT)
    window.set_focus()
    return app, window


def _choose_plane(selection_window, label_text: str) -> None:
    selection_window.child_window(title=label_text, control_type="RadioButton").select()


def _choose_scenario(selection_window, index: int) -> None:
    combo = ComboBoxWrapper(selection_window.child_window(control_type="ComboBox"))
    combo.select(index)


def _start_game(selection_window) -> object:
    selection_window.child_window(title="Iniciar partida", control_type="Button").click_input()
    game_window = selection_window.app.window(title_re=".*Juego de Aviones.*")
    game_window.wait("visible", timeout=STARTUP_TIMEOUT)
    game_window.set_focus()
    return game_window


def _read_player_life(game_window) -> int:
    label = game_window.child_window(title_re="Vida:.*", control_type="Text")
    texto = label.window_text()
    try:
        return int(texto.split(":", maxsplit=1)[1].strip())
    except (IndexError, ValueError) as exc:  # pragma: no cover - defensivo
        raise AssertionError(f"No se pudo interpretar la vida del jugador desde '{texto}'") from exc


def _drive_toward_enemy(game_window, duration: float = 5.0) -> None:
    end_time = time.time() + duration
    while time.time() < end_time:
        send_keys("{UP}{LEFT}{RIGHT}")
        time.sleep(0.1)


def _wait_for_life_delta(game_window, expected_delta: int, timeout: float) -> tuple[int, int]:
    vida_anterior = _read_player_life(game_window)
    end_time = time.time() + timeout
    while time.time() < end_time:
        _drive_toward_enemy(game_window, duration=0.5)
        vida_actual = _read_player_life(game_window)
        if vida_anterior - vida_actual == expected_delta:
            return vida_anterior, vida_actual
        vida_anterior = vida_actual
    raise AssertionError(f"No se observó un cambio de vida de {expected_delta} puntos dentro del tiempo esperado")


@WINDOWS_ONLY
def test_selection_window_launches_and_controls_present():
    app, window = _launch_selection_app()
    try:
        assert window.child_window(title="Blindado (vida 100 - robusto)", control_type="RadioButton").exists()
        assert window.child_window(title="Equilibrado (vida 70 - balance)", control_type="RadioButton").exists()
        assert window.child_window(title="Ligero (vida 50 - ágil)", control_type="RadioButton").exists()
        combo = window.child_window(control_type="ComboBox")
        assert combo.exists(), "El ComboBox de escenarios debe estar disponible"
    finally:
        app.kill()


@WINDOWS_ONLY
def test_airplane_radio_buttons_toggle_independently():
    app, window = _launch_selection_app()
    try:
        _choose_plane(window, "Ligero (vida 50 - ágil)")
        assert window.child_window(title="Ligero (vida 50 - ágil)", control_type="RadioButton").get_toggle_state()

        _choose_plane(window, "Equilibrado (vida 70 - balance)")
        assert window.child_window(title="Equilibrado (vida 70 - balance)", control_type="RadioButton").get_toggle_state()
        assert not window.child_window(title="Ligero (vida 50 - ágil)", control_type="RadioButton").get_toggle_state()
    finally:
        app.kill()


@WINDOWS_ONLY
def test_scenario_combo_selection_changes_value():
    app, window = _launch_selection_app()
    try:
        _choose_scenario(window, 1)
        combo = ComboBoxWrapper(window.child_window(control_type="ComboBox"))
        assert combo.selected_text() == "Escenario 2"

        _choose_scenario(window, 3)
        assert combo.selected_text() == "Escenario 4"
    finally:
        app.kill()


@WINDOWS_ONLY
def test_start_game_closes_selection_and_opens_main_game_window():
    app, window = _launch_selection_app()
    try:
        game_window = _start_game(window)
        assert not window.is_visible(), "La ventana de selección debe ocultarse al iniciar la partida"
        assert game_window.exists(), "La ventana principal del juego debe abrirse"
    finally:
        app.kill()


@WINDOWS_ONLY
def test_space_key_can_be_pressed_without_crashing_game():
    app, window = _launch_selection_app()
    try:
        game_window = _start_game(window)
        send_keys("{SPACE 5}")
        assert game_window.exists() and game_window.is_visible(), "La ventana del juego debe seguir activa tras disparar"
    finally:
        app.kill()


@WINDOWS_ONLY
def test_enemy_collision_reduces_life_by_ten_points():
    app, window = _launch_selection_app()
    try:
        _choose_plane(window, "Ligero (vida 50 - ágil)")
        game_window = _start_game(window)
        _drive_toward_enemy(game_window, duration=3)
        vida_inicial, vida_actual = _wait_for_life_delta(game_window, expected_delta=10, timeout=GAME_READY_TIMEOUT)
        assert vida_inicial - vida_actual == 10
    finally:
        app.kill()


@WINDOWS_ONLY
def test_obstacle_collision_reduces_life_by_fifteen_points():
    app, window = _launch_selection_app()
    try:
        _choose_plane(window, "Ligero (vida 50 - ágil)")
        _choose_scenario(window, 0)
        game_window = _start_game(window)
        end_time = time.time() + GAME_READY_TIMEOUT
        vida_anterior = _read_player_life(game_window)

        while time.time() < end_time:
            send_keys("{LEFT}{RIGHT}{UP}")
            time.sleep(0.2)
            vida_actual = _read_player_life(game_window)
            if vida_anterior - vida_actual == 15:
                break
            vida_anterior = vida_actual
        else:
            raise AssertionError("No se observó una reducción de 15 puntos por colisión con obstáculo")
    finally:
        app.kill()


@WINDOWS_ONLY
def test_background_changes_with_selected_scenario():
    first_app, first_window = _launch_selection_app()
    try:
        _choose_scenario(first_window, 0)
        first_game = _start_game(first_window)
        time.sleep(1)
        first_image_hash = hash(first_game.capture_as_image().tobytes())
    finally:
        first_app.kill()

    second_app, second_window = _launch_selection_app()
    try:
        _choose_scenario(second_window, 3)
        second_game = _start_game(second_window)
        time.sleep(1)
        second_image_hash = hash(second_game.capture_as_image().tobytes())
    finally:
        second_app.kill()

    assert first_image_hash != second_image_hash, "El fondo debe cambiar cuando se selecciona otro nivel"

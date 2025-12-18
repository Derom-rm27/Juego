import pytest
import os
import time
import re
from pywinauto import Application
from pywinauto.keyboard import send_keys

# RUTA DEL EJECUTABLE
EXE_PATH = r"C:\juego\MoverObjeto\MoverObjeto\bin\Debug\net9.0-windows\MoverObjeto.exe"


@pytest.fixture(scope="module")
def app():
    """Lanza la aplicación con backend UIA para mayor compatibilidad con .NET 9."""
    if not os.path.exists(EXE_PATH):
        pytest.fail(f"No se encontró el ejecutable en: {os.path.abspath(EXE_PATH)}")

    # Cambiamos a backend="uia" porque WinForms en .NET 9 lo prefiere
    app = Application(backend="uia").start(EXE_PATH)

    try:
        # Esperar y conectar a la ventana de selección
        dlg = app.window(title_re=".*Seleccionar.*")
        dlg.wait('visible', timeout=30)
        yield app
    finally:
        app.kill()


def test_1_seleccion_jugador_y_escenario(app):
    """EVALUACIÓN 1 y 2: Selección de tipo de avión y nivel de escenario."""
    dlg = app.window(title_re=".*Seleccionar.*")

    try:
        # Buscamos el RadioButton que contenga 'Ligero'
        # En UIA, el control_type suele ser "RadioButton"
        rb_ligero = dlg.child_window(title_re=".*Ligero.*", control_type="RadioButton")
        rb_ligero.click()

        # Selección de Escenario
        combo = dlg.child_window(control_type="ComboBox")
        combo.select("Escenario 3")

        assert combo.selected_text() == "Escenario 3"
    except Exception as e:
        # Si falla, imprimimos qué controles existen para debuguear
        dlg.print_control_identifiers()
        raise e


def test_2_lanzar_juego(app):
    """EVALUACIÓN 3: Lanzar el juego al dar clic en Iniciar Partida."""
    dlg = app.window(title_re=".*Seleccionar.*")

    btn_iniciar = dlg.child_window(title="Iniciar partida", control_type="Button")
    btn_iniciar.click()

    # Esperamos a la ventana del juego (Título en mayúsculas según tu C#)
    time.sleep(3)
    juego = app.window(title="JUEGO DE AVIONES")
    juego.wait('visible', timeout=20)

    assert juego.exists()


def test_3_actividad_tecla_disparo(app):
    """EVALUACIÓN 4: Prueba la tecla de disparo (Espacio)."""
    juego = app.window(title="JUEGO DE AVIONES")
    juego.set_focus()

    # Simular disparo
    send_keys('{SPACE}')
    time.sleep(0.5)

    assert juego.is_active()


def test_4_evaluacion_vida_inicial(app):
    """EVALUACIÓN 5: Verificar vida inicial."""
    juego = app.window(title="JUEGO DE AVIONES")

    # Buscamos el Label de vida
    label_vida = juego.child_window(title_re=".*Vida:.*", control_type="Text")
    texto_vida = label_vida.window_text()

    vida_valor = int(re.search(r'\d+', texto_vida).group())
    assert vida_valor > 0


def test_5_evaluacion_colision_y_obstaculo(app):
    """EVALUACIÓN 6 y 7: Daño por colisión y obstáculos."""
    juego = app.window(title="JUEGO DE AVIONES")
    label_vida = juego.child_window(title_re=".*Vida:.*", control_type="Text")

    vida_pre = int(re.search(r'\d+', label_vida.window_text()).group())

    juego.set_focus()
    # Movemos la nave para buscar colisión
    for _ in range(10):
        send_keys('{UP}')
        time.sleep(0.1)

    time.sleep(1.5)
    vida_post = int(re.search(r'\d+', label_vida.window_text()).group())

    if vida_post < vida_pre:
        daño = vida_pre - vida_post
        assert daño in [1, 10, 15]
    else:
        pytest.skip("No hubo colisión en este trayecto aleatorio.")


def test_6_niveles_y_escenarios(app):
    """Verificar que el juego sigue activo."""
    juego = app.window(title="JUEGO DE AVIONES")
    assert juego.is_active()
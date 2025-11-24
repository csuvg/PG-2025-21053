# PG-2025-21053
Proyecto de Graduación 2025 - Carnet: 21053

# Entrenamiento de un agente artificial en un entorno dinámico controlado por el jugador, utilizando aprendizaje por refuerzo en Unity

## Descripción
MazeMaster AI es un proyecto de juego híbrido desarrollado en Unity que permite a los usuarios diseñar entornos dinámicos (laberintos con obstáculos, enemigos y elementos interactivos) para entrenar un agente de inteligencia artificial mediante aprendizaje por refuerzo (RL). El problema que resuelve es la brecha entre la edición creativa de niveles y el entrenamiento autónomo de IA, permitiendo a los jugadores iterar diseños mientras la IA aprende a navegar, resolver puzzles y combatir, con el objetivo de demostrar viabilidad de RL en entornos controlados por el usuario.

## Tecnologías Utilizadas
- Unity (versión 2022+ para compatibilidad con ML-Agents)
- ML-Agents Toolkit (para entrenamiento de IA con aprendizaje por refuerzo)
- C# (para scripts de lógica, UI y eventos)
- EventBus (sistema personalizado de mensajería desacoplada)
- NavMesh (para pathfinding de enemigos)
- Python (para ejecución de entrenamiento vía ML-Agents CLI)

## Requisitos Previos
- Unity 2022.3 o superior (con ML-Agents package instalado vía Package Manager)
- Python 3.8+ con Miniconda/Anaconda (entorno virtual para ML-Agents)
- ML-Agents CLI instalado (`pip install mlagents`)
- Git para clonar el repositorio
- Hardware con GPU recomendada (e.g., NVIDIA RTX para entrenamiento acelerado)

## Instalación
1. Clona el repositorio:
   ```
   git clone https://github.com/tu-usuario/MazeMasterAI.git
   cd MazeMasterAI
   ```
2. Abre el proyecto en Unity:
   - Lanza Unity Hub, selecciona "Add" y apunta a la carpeta clonada.
   - Instala ML-Agents package: Window > Package Manager > Busca "ML-Agents" > Install (versión 2.0+ recomendada).
3. Configura el entorno Python para entrenamiento:
   ```
   conda create -n mlagents python=3.10
   conda activate mlagents
   pip install mlagents
   ```
4. Ejecuta el juego en Editor: Abre escena principal (e.g., LevelSelector), presiona Play.
5. Para entrenamiento: En terminal, ejecuta `mlagents-learn config/RLAgent_config.yaml --run-id=TestRun --num-envs=1`, luego presiona Play en Unity para conectar.

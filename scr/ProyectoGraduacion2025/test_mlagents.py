import torch
import gym
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.base_env import ActionTuple

print("PyTorch version:", torch.__version__)
print("GPU disponible:", torch.cuda.is_available())

# Probar Gym
env = gym.make("CartPole-v1")
obs = env.reset()
obs, reward, terminated, truncated, info = env.step(env.action_space.sample())
print("Gym CartPole paso OK")

# Probar ML-Agents (sin archivo ejecutable Unity real)
try:
    env = UnityEnvironment(file_name=None, no_graphics=True)
    env.reset()
    print("ML-Agents import OK")
except Exception as e:
    print("ML-Agents test:", e)

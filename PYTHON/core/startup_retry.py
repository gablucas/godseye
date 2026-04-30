import asyncio
from fastapi import FastAPI
import httpx

from core.godseyedata_loader import (
    load_godseye_data_from_api,
    GodsEyeLoadError
)
from core.initializer import initialize_monitoring


async def load_godseye_with_retry(app: FastAPI):
    retry = 0
    print("🔥 load_godseye_with_retry STARTOU")

    while True:
        try:
            print("🔄 Tentando buscar dados do GodsEye...")

            data = await load_godseye_data_from_api()
            retry = 0  # reset após sucesso

            print("✅ Dados carregados com sucesso")
            await initialize_monitoring(app, data)
            return

        except asyncio.CancelledError:
            print("🛑 load_godseye_with_retry cancelado")
            raise

        except (httpx.ConnectError, httpx.ReadTimeout):
            retry += 1
            wait = min(5 * retry, 30)
            print(f"⚠️ API .NET offline. Tentando novamente em {wait}s...")
            await asyncio.sleep(wait)

        except GodsEyeLoadError as e:
            print(f"❌ Erro lógico da API: {e}")
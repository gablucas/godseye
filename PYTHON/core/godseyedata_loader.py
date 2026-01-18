import json
import httpx

class GodsEyeLoadError(Exception):
    pass


async def load_godseye_data_from_api():
    async with httpx.AsyncClient(verify=False, timeout=10) as client:
        response = await client.get(
            "https://localhost:7010/api/godseye"
        )

    response.raise_for_status()
    result = response.json()

    if result.get("sucesso") is not True:
        raise GodsEyeLoadError(
            "Não foi possível buscar os dados para monitoramento"
        )

    result_data = result.get("dados")

    if not result_data or "data" not in result_data:
        raise GodsEyeLoadError(
            "Resposta inválida da API (.NET): campo 'data' ausente"
        )

    raw_data = result_data["data"]

    if isinstance(raw_data, str):
        try:
            raw_data = json.loads(raw_data)
        except json.JSONDecodeError:
            raise GodsEyeLoadError(
                "Campo 'data' não é um JSON válido"
            )

    return raw_data

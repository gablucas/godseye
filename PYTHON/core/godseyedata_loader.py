import json
import httpx

class GodsEyeLoadError(Exception):
    pass


async def load_godseye_data_from_api():
    async with httpx.AsyncClient(verify=False, timeout=10) as client:
        response = await client.get(
            "https://localhost:7010/api/godseye"
        )

    # Valida o status HTTP
    #200, 201, etc → continua
    #400, 401, 404, 500 → lança exceção automaticamente
    response.raise_for_status() 
    result = response.json()

    result_data = result["data"]

    # Se vier como string (seu caso atual)
    if isinstance(result_data, str):
        try:
            result_data = json.loads(result_data)
        except json.JSONDecodeError:
            raise GodsEyeLoadError(
                "Campo 'data' não é um JSON válido"
            )

    return result_data

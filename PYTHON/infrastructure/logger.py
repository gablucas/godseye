# infrastructure/logger.py

import httpx
from config.settings import VERIFY_SSL
from schemas.environment_monitoring_schema import EnvironmentMonitoringCreateRequest
from schemas.dwell_time_monitoring import DwellTimeMonitoringCreateRequest


class LogSender:

    @staticmethod
    def dotnet_create_environment_monitoring_log(
        result: EnvironmentMonitoringCreateRequest
    ):
        print('ENVIANDO LOG')

        with httpx.Client(
            verify=VERIFY_SSL,
            timeout=10
        ) as client:
            response = client.post(
                "https://localhost:7010/api/environmentmonitoring",
                json=result.model_dump(by_alias=True, mode="json")
            )
            response.raise_for_status()

    @staticmethod
    def dotnet_create_dwell_time_monitoring_log(
        result: DwellTimeMonitoringCreateRequest
    ):
        with httpx.Client(
            verify=VERIFY_SSL,
            timeout=10
        ) as client:
            response = client.post(
                "https://localhost:7010/api/incidentrecording/process/done",
                json=result.model_dump(by_alias=True, mode="json")
            )
            response.raise_for_status()

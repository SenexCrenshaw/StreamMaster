import { useCurrentSettingRequest } from '@lib/redux/hooks/currentSettingRequest';
import { useUpdateSettingRequest } from '@lib/redux/hooks/updateSettingRequest';
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

export function useSettingChangeHandler() {
  const { currentSettingRequest, setCurrentSettingRequest } = useCurrentSettingRequest('CurrentSettingDto');
  const { updateSettingRequest, setUpdateSettingRequest } = useUpdateSettingRequest('UpdateSettingRequest');

  const onChange = (existing: SettingDto | null, updateSettingValue: SettingDto | null) => {
    if (updateSettingValue !== null) {
      let mergedMain = updateSettingValue;
      if (updateSettingRequest?.parameters !== undefined) {
        mergedMain = {
          ...updateSettingRequest,
          ...updateSettingValue
        };
      }

      let mergedSdSettings = updateSettingValue?.SDSettings;
      if (updateSettingRequest?.parameters?.SDSettings) {
        mergedSdSettings = {
          ...updateSettingRequest.parameters.SDSettings,
          ...updateSettingValue.SDSettings
        };
      }
      if (mergedSdSettings) {
        mergedMain.SDSettings = mergedSdSettings;
      }

      const request = { parameters: mergedMain } as UpdateSettingRequest;
      setUpdateSettingRequest(request);
    }

    if (existing !== null && currentSettingRequest !== null) {
      setCurrentSettingRequest(existing);
    }
  };

  return { currentSettingRequest, onChange, updateSettingRequest };
}

import { useCurrentSettingRequest } from '@lib/redux/hooks/currentSettingRequest';
import { useUpdateSettingRequest } from '@lib/redux/hooks/updateSettingRequest';
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

export function useSettingChangeHandler() {
  const { currentSettingRequest, setCurrentSettingRequest } = useCurrentSettingRequest('CurrentSettingDto');
  const { updateSettingRequest, setUpdateSettingRequest } = useUpdateSettingRequest('UpdateSettingRequest');

  const onChange = (existing: SettingDto | null, updatedValues: UpdateSettingRequest | null) => {
    if (updatedValues !== null && updateSettingRequest != null) {
      const mergedMain = {
        ...updateSettingRequest,
        ...updatedValues
      };

      const mergedSdSettings = {
        ...updateSettingRequest.parameters.SDSettings,
        ...updatedValues.parameters.SDSettings
      };

      mergedMain.parameters.SDSettings = mergedSdSettings;

      setUpdateSettingRequest(mergedMain);
    }

    if (existing !== null && currentSettingRequest !== null) {
      setCurrentSettingRequest(existing);
    }
  };

  return { currentSettingRequest, onChange, updateSettingRequest };
}

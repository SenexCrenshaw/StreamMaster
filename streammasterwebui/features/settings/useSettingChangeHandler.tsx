import { SettingDto, UpdateSettingRequest } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { useSelectUpdateSettingRequest } from '@lib/redux/slices/selectedUpdateSettingRequestSlice';

export function useSettingChangeHandler() {
  const { selectedCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');
  const { selectUpdateSettingRequest, setSelectedUpdateSettingRequest } = useSelectUpdateSettingRequest('UpdateSettingRequest');

  const onChange = (existing: SettingDto | null, updatedValues: UpdateSettingRequest | null) => {
    if (updatedValues !== null && selectUpdateSettingRequest !== null) {
      const updated = { ...selectUpdateSettingRequest, ...updatedValues };
      setSelectedUpdateSettingRequest(updated);
    }

    if (existing !== null && selectedCurrentSettingDto !== null) {
      setSelectedCurrentSettingDto(existing);
    }
  };

  return { onChange, selectUpdateSettingRequest, selectedCurrentSettingDto };
}

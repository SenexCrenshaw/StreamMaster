import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import useSettings from '@lib/useSettings';
import { Dropdown } from 'primereact/dropdown';
import { memo, useEffect, useMemo, useState } from 'react';

interface FFMPEGProfileDropDownProperties {
  readonly id: string;
  readonly onChange: (FfmpegProfileId: string) => void;
}

const FFMPEGProfileDropDown = ({ id, onChange }: FFMPEGProfileDropDownProperties): JSX.Element => {
  const [FfmpegProfile, setFfmpegProfile] = useState<string | null>(null);
  const settings = useSettings();
  const { selectedStreamGroup } = useSelectedStreamGroup(id);

  useEffect(() => {
    if (selectedStreamGroup === undefined) {
      return;
    }

    if (selectedStreamGroup.ffmpegProfileId !== undefined && selectedStreamGroup.ffmpegProfileId !== '') {
      setFfmpegProfile(selectedStreamGroup.ffmpegProfileId);
    }
  }, [selectedStreamGroup]);

  const profiles = useMemo(() => {
    if (settings.data.ffmpegProfiles === undefined) {
      return [];
    }

    const toRet = Object.keys(settings.data.ffmpegProfiles).map((key) => {
      return {
        label: key,
        value: key
      };
    });
    return toRet;
  }, [settings.data.ffmpegProfiles]);

  console.log(FfmpegProfile);
  return (
    <Dropdown
      className="bordered-text w-full"
      options={profiles}
      value={FfmpegProfile}
      onChange={(e) => {
        setFfmpegProfile(e.value);
        console.log('e', e);
        onChange(e.value);
      }}
    />
  );
};

export default memo(FFMPEGProfileDropDown);

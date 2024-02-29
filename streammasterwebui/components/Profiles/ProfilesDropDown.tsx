import { useProfilesGetFfmpegProfilesQuery } from '@lib/iptvApi';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { Dropdown } from 'primereact/dropdown';
import { memo, useEffect, useMemo, useState } from 'react';

interface ProfilesDropDownProperties {
  readonly id: string;
  readonly onChange: (FfmpegProfileId: string) => void;
}

const ProfilesDropDown = ({ id, onChange }: ProfilesDropDownProperties): JSX.Element => {
  const [FfmpegProfile, setFfmpegProfile] = useState<string | null>(null);
  const settingsQuery = useProfilesGetFfmpegProfilesQuery();
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
    if (settingsQuery.data === undefined) {
      return [];
    }

    const toRet = Object.keys(settingsQuery.data).map((key) => {
      return {
        label: key,
        value: key
      };
    });
    return toRet;
  }, [settingsQuery.data]);

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

export default memo(ProfilesDropDown);

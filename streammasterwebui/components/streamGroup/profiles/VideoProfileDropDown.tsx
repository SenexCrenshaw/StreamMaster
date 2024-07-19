import SMDropDown from '@components/sm/SMDropDown';
import { Logger } from '@lib/common/logger';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { VideoOutputProfileDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, useCallback, useMemo } from 'react';

interface VideoProfileDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly value: string;
  readonly onChange: (value: any) => void;
}

const VideoProfileDropDown = ({ buttonDarkBackground = false, onChange, value }: VideoProfileDropDownProps) => {
  const { data, isLoading } = useGetVideoProfiles();

  const selectedVideoProfile = useMemo(() => {
    if (!data || !value) {
      return undefined;
    }

    return data.find((x) => x.ProfileName === value);
  }, [data, value]);

  Logger.debug('VideoProfileDropDown', { data: data, selectedVideoProfile, value });

  const itemTemplate = useCallback((option: VideoOutputProfileDto): JSX.Element => {
    return <div className="text-xs text-container">{option?.ProfileName ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    return (
      <div className="text-container" style={{ paddingLeft: '0.24rem' }}>
        {selectedVideoProfile?.ProfileName ?? ''}
      </div>
    );
  }, [selectedVideoProfile]);

  return (
    <SMDropDown
      isOverLayLoading={isLoading}
      buttonDarkBackground={buttonDarkBackground}
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={data}
      dataKey="ProfileName"
      info=""
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? 'Video Profile' : undefined}
      onChange={onChange}
      propertyToMatch="ProfileName"
      scrollHeight="20vh"
      title="Profiles"
      value={selectedVideoProfile}
      zIndex={12}
    />
  );
};

export default VideoProfileDropDown;

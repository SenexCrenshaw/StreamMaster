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
    if (!data) {
      return undefined;
    }

    const ret = data.find((x) => x.Name === value);
    Logger.debug('VideoProfileDropDown', data, value, ret);

    return data.find((x) => x.Name === value);
  }, [data, value]);

  const itemTemplate = useCallback((option: VideoOutputProfileDto): JSX.Element => {
    return <div className="text-xs text-container">{option?.Name ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    return (
      <div className="sm-epg-selector">
        <div className="text-container" style={{ paddingLeft: '0.12rem' }}>
          {selectedVideoProfile?.Name ?? ''}
        </div>
      </div>
    );
  }, [selectedVideoProfile?.Name]);

  return (
    <SMDropDown
      isOverLayLoading={isLoading}
      buttonDarkBackground={buttonDarkBackground}
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={data}
      dataKey="label"
      info=""
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? 'Video Profile' : undefined}
      onChange={onChange}
      optionValue="label"
      scrollHeight="20vh"
      title="Profiles"
      value={selectedVideoProfile}
    />
  );
};

export default VideoProfileDropDown;

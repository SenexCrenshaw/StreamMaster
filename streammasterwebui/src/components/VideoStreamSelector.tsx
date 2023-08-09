
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import React from 'react';
import { type VideoStreamsGetVideoStreamsApiArg } from '../store/iptvApi';
import { useVideoStreamsGetVideoStreamsQuery, type VideoStreamDto } from '../store/iptvApi';

export const VideoStreamSelector = (props: VideoStreamSelectorProps) => {
  const elementRef = React.useRef(null)
  const [selectedVideoStream, setSelectedVideoStream] = React.useState<VideoStreamDto>({} as VideoStreamDto);

  const videoStreamsQuery = useVideoStreamsGetVideoStreamsQuery({} as VideoStreamsGetVideoStreamsApiArg);

  React.useEffect(() => {
    if (props.value !== undefined && props.value !== selectedVideoStream.user_Tvg_ID) {
      const v = videoStreamsQuery.data?.data.find((a: VideoStreamDto) => a.user_Tvg_name === props.value);
      if (v)
        setSelectedVideoStream(v);
    }
  }, [props.value, selectedVideoStream, videoStreamsQuery.data]);

  const isDisabled = React.useMemo((): boolean => {
    if (videoStreamsQuery.isLoading) {
      return true;
    }

    return false;
  }, [videoStreamsQuery.isLoading]);


  const onDropdownChange = (sg: VideoStreamDto) => {
    if (!sg) return;

    setSelectedVideoStream(sg);
    props?.onChange?.(sg);
  };

  const getOptions = React.useMemo((): SelectItem[] => {
    if (!videoStreamsQuery.data)
      return [
        {
          label: 'Loading...',
          value: {} as VideoStreamDto,
        } as SelectItem,
      ];

    const ret = videoStreamsQuery.data?.data?.filter((a) => !a.isHidden)
      .sort((a, b) => a.user_Tvg_name.localeCompare(b.user_Tvg_name))
      .map((a) => {
        return { label: a.user_Tvg_name, value: a } as SelectItem;
      });

    return ret;
  }, [videoStreamsQuery.data]);

  return (
    <div className='flex w-full justify-items-center border=1'  >
      <Dropdown
        className="VideoStreamselector w-full"
        disabled={isDisabled}
        filter
        onChange={(e) => onDropdownChange(e.value)}
        options={getOptions}
        placeholder="Video Streams"
        ref={elementRef}
        value={selectedVideoStream}
      />
    </div>
  );
};

VideoStreamSelector.displayName = 'VideoStreamSelector';
VideoStreamSelector.defaultProps = {

};
type VideoStreamSelectorProps = {
  onChange: ((value: VideoStreamDto) => void);
  value: string | undefined;
};

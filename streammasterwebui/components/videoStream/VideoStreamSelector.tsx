import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { useEffect, useMemo, useRef, useState } from 'react';

export const VideoStreamSelector = (props: VideoStreamSelectorProperties) => {
  const elementReference = useRef(null);
  const [selectedVideoStreamIdName, setSelectedVideoStreamIdName] = useState<IdName | undefined>();

  const videoStreamsQuery = useVideoStreamsGetVideoStreamNamesQuery();

  useEffect(() => {
    if (props.value !== undefined && videoStreamsQuery.data && props.value !== selectedVideoStreamIdName?.name) {
      const v = videoStreamsQuery.data.find((idName: IdName) => idName.name === props.value);

      if (v) setSelectedVideoStreamIdName(v);
    }
  }, [props.value, selectedVideoStreamIdName?.name, videoStreamsQuery.data]);

  const isDisabled = useMemo((): boolean => {
    if (videoStreamsQuery.isLoading) {
      return true;
    }

    return false;
  }, [videoStreamsQuery.isLoading]);

  const onDropdownChange = (sg: IdName) => {
    if (!sg) return;

    setSelectedVideoStreamIdName(sg);
    props?.onChange?.(sg);
  };

  const getOptions = useMemo((): SelectItem[] => {
    if (!videoStreamsQuery.data) {
      return [
        {
          label: 'Loading...',
          value: {} as VideoStreamDto
        } as SelectItem
      ];
    }

    const returnValue = [...videoStreamsQuery.data].sort((a, b) => a.name.localeCompare(b.name)).map((a) => ({ label: a.name, value: a } as SelectItem));

    return returnValue;
  }, [videoStreamsQuery.data]);

  return (
    <div className="flex w-full justify-items-center border=1">
      <Dropdown
        filterInputAutoFocus
        className="VideoStreamselector w-full"
        disabled={isDisabled}
        filter
        onChange={(e) => onDropdownChange(e.value)}
        options={getOptions}
        placeholder="Video Streams"
        ref={elementReference}
        value={selectedVideoStreamIdName}
      />
    </div>
  );
};

VideoStreamSelector.displayName = 'VideoStreamSelector';

interface VideoStreamSelectorProperties {
  readonly onChange: (value: IdName) => void;
  readonly value: string | undefined;
}

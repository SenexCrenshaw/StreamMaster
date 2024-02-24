import StandardHeader from '@components/StandardHeader';
import { VideoPlayerIcon } from '@lib/common/icons';
import { IdNameUrl, useVideoStreamsGetVideoStreamNamesAndUrlsQuery } from '@lib/iptvApi';
import { MediaPlayer, MediaProvider } from '@vidstack/react';
import { DefaultVideoLayout, defaultLayoutIcons } from '@vidstack/react/player/layouts/default';
import '@vidstack/react/player/styles/default/layouts/video.css';
import '@vidstack/react/player/styles/default/theme.css';
import { Dropdown, DropdownChangeEvent } from 'primereact/dropdown';
import React, { useMemo } from 'react';

const VideoPlayer = () => {
  const [selectedID, setSelectedID] = React.useState<IdNameUrl | null>(null);
  const [title, setTitle] = React.useState<string>('');
  const [src, setSrc] = React.useState<string>('');
  const [dropdownFocus, setDropdownFocus] = React.useState<boolean>(false);
  const [showDropdown, setShowDropdown] = React.useState<boolean>(true);
  const hideDropdownTimeoutRef = React.useRef<NodeJS.Timeout | null>(null);

  const namesAndUrlsQuery = useVideoStreamsGetVideoStreamNamesAndUrlsQuery();

  const namesAndUrls = useMemo(() => {
    return namesAndUrlsQuery.data?.map((item) => ({ name: item.name, value: item })) || [];
  }, [namesAndUrlsQuery.data]);

  const handleMouseMove = (show: boolean) => {
    if (dropdownFocus) {
      if (hideDropdownTimeoutRef.current) {
        clearTimeout(hideDropdownTimeoutRef.current);
      }
      return true;
    }

    setShowDropdown(show);
    if (hideDropdownTimeoutRef.current) {
      clearTimeout(hideDropdownTimeoutRef.current);
    }
    hideDropdownTimeoutRef.current = setTimeout(() => {
      setShowDropdown(false);
    }, 5000);
  };

  return (
    <StandardHeader className="videoPlayer flex flex-column h-full" displayName="Video" icon={<VideoPlayerIcon />}>
      <MediaPlayer
        title={title}
        src={src}
        autoPlay
        onControlsChange={(e) => {
          handleMouseMove(e as boolean);
        }}
      >
        <MediaProvider />
        {showDropdown && (
          <div className="flex absolute top-0 left-0 justify-content-center h-full w-full p-0 m-0 w-full z-5">
            <Dropdown
              onShow={() => {
                console.log('focus');
                setDropdownFocus(true);
              }}
              onHide={() => {
                console.log('onBlur');
                setDropdownFocus(false);
              }}
              filter
              optionLabel="name"
              value={selectedID}
              onChange={(e: DropdownChangeEvent) => {
                setTitle(e.value.name);
                setSrc(e.value.url);
                setSelectedID(e.value);
              }}
              options={namesAndUrls}
              placeholder="Select a Stream"
              virtualScrollerOptions={{
                itemSize: 32,
                scrollHeight: '40vh',
                style: {
                  width: '300px'
                }
              }}
            />
          </div>
        )}
        <DefaultVideoLayout icons={defaultLayoutIcons} />
      </MediaPlayer>
    </StandardHeader>
  );
};

VideoPlayer.displayName = 'VideoPlayer';

export default React.memo(VideoPlayer);

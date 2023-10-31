import ChannelHandlerSelector from '@components/ChannelHandlerSelector';
import AddButton from '@components/buttons/AddButton';
import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import EPGSelector from '@components/selectors/EPGSelector';
import IconSelector from '@components/selectors/IconSelector';
import { getIconUrl } from '@lib/common/common';
import { CreateVideoStreamRequest, UpdateVideoStreamRequest, VideoStreamDto, VideoStreamHandlers } from '@lib/iptvApi';
import useSettings from '@lib/useSettings';
import { Accordion, AccordionTab } from 'primereact/accordion';
import { InputNumber } from 'primereact/inputnumber';
import { InputText } from 'primereact/inputtext';
import { memo, useEffect, useMemo, useState } from 'react';
import InputWrapper from './InputWrapper';
import VideoStreamDataSelector from './VideoStreamDataSelector';
import VideoStreamSelectedVideoStreamDataSelector from './VideoStreamSelectedVideoStreamDataSelector';

interface VideoStreamPanelProperties {
  readonly group?: string;
  readonly onEdit?: (e: UpdateVideoStreamRequest) => void;
  readonly onSave?: (e: CreateVideoStreamRequest) => void;
  readonly videoStream?: VideoStreamDto | undefined;
}

const VideoStreamPanel = ({ group, onEdit, onSave, videoStream }: VideoStreamPanelProperties) => {
  const settings = useSettings();
  const [name, setName] = useState<string>('');
  const [url, setUrl] = useState<string>('');

  const [iconSource, setIconSource] = useState<string>('');
  const [videoStreams, setVideoStreams] = useState<VideoStreamDto[] | undefined>();
  const [channelNumber, setChannelNumber] = useState<number>(0);
  const [channelHandler, setChannelHandler] = useState<VideoStreamHandlers>(0);
  const [epgId, setEpgId] = useState<string>('');
  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [lastActiveIndex, setLastActiveIndex] = useState<number>(0);
  const [channelGroup, setChannelGroup] = useState<string | undefined>();

  const [dataSource, setDataSource] = useState<VideoStreamDto[] | undefined>();

  useEffect(() => {
    if (group) {
      setChannelGroup(group);
    }
  }, [group]);

  useEffect(() => {
    const {
      childVideoStreams,
      user_Tvg_name: userTvgName,
      user_Url: userUrl,
      user_Tvg_logo: userTvgLogo,
      user_Tvg_chno: userTvgChno,
      user_Tvg_ID: userTvgId,
      videoStreamHandler,
      user_Tvg_group: userTvgGroup
    } = videoStream ?? {};

    if (childVideoStreams) {
      setVideoStreams(childVideoStreams);
    }

    if (userTvgName) {
      setName(userTvgName);
    }

    if (userUrl) {
      setUrl(userUrl);
    }

    if (userTvgLogo) {
      setIconSource(userTvgLogo);
    }

    if (userTvgChno) {
      setChannelNumber(userTvgChno);
    }

    if (userTvgId) {
      setEpgId(userTvgId);
    }

    if (videoStreamHandler) {
      setChannelHandler(videoStreamHandler);
    }

    if (userTvgGroup) {
      setChannelGroup(userTvgGroup);
    }
  }, [videoStream]);

  const isSaveEnabled = useMemo((): boolean => {
    if (!videoStream) {
      return name !== '';
    }

    if (videoStream.user_Tvg_name !== name) {
      return true;
    }

    if (videoStream.user_Tvg_chno !== channelNumber) {
      return true;
    }

    if (videoStream.user_Tvg_logo !== iconSource) {
      return true;
    }

    if (videoStream.user_Url !== url) {
      return true;
    }

    return false;
  }, [channelNumber, iconSource, name, videoStream, url]);

  const onsetActiveIndex = (index: number) => {
    if (index === null) {
      index = lastActiveIndex === 0 ? 1 : 0;
    }

    setLastActiveIndex(index);
    setActiveIndex(index);
  };

  return (
    <Accordion activeIndex={activeIndex} className="w-full h-full" onTabChange={(e) => onsetActiveIndex(e.index as number)}>
      <AccordionTab className="w-full h-full" header="General">
        <div className="grid flex justify-content-start align-items-center surface-overlay m-0">
          <div className="flex col-12 justify-content-start align-items-center p-0 m-0">
            {/* Image */}
            <div className="flex col-2 justify-content-center align-items-center">
              <img alt={iconSource ?? 'Logo'} className="icon-template-large" src={getIconUrl(iconSource, settings.defaultIcon, settings.cacheIcon)} />
            </div>

            <div className="flex flex-wrap col-10 left_border justify-content-between">
              <div className="flex col-12 justify-content-between">
                <InputWrapper
                  columnSize={6}
                  label="Name"
                  renderInput={() => (
                    <InputText
                      autoFocus
                      className="w-full bordered-text mr-2"
                      onChange={(e) => setName(e.target.value)}
                      placeholder="Name"
                      type="text"
                      value={name}
                    />
                  )}
                />

                <InputWrapper
                  columnSize={4}
                  label="Logo"
                  renderInput={() => <IconSelector className="w-full bordered-text mr-2" onChange={setIconSource} value={iconSource} />}
                />

                <InputWrapper
                  columnSize={2}
                  label="Ch. #"
                  renderInput={() => (
                    <InputNumber
                      className="w-full"
                      id="channelNumber"
                      max={999_999}
                      min={0}
                      onChange={(e) => {
                        setChannelNumber(e.value ?? 0);
                      }}
                      size={3}
                      value={channelNumber}
                    />
                  )}
                />
              </div>

              <div className="flex col-12">
                <InputWrapper
                  columnSize={6}
                  label="EPG"
                  renderInput={() => <EPGSelector className="w-full bordered-text mr-2" onChange={setEpgId} value={epgId} />}
                />

                <InputWrapper
                  columnSize={6}
                  label="Group"
                  renderInput={() => <ChannelGroupSelector className="w-full bordered-text" onChange={(e) => setChannelGroup(e)} value={channelGroup} />}
                />
              </div>

              <div className="flex col-12 justify-content-between align-items-center">
                <InputWrapper
                  columnSize={11}
                  label="Stream URL"
                  renderInput={() => (
                    <InputText className="w-full bordered-text" onChange={(e) => setUrl(e.target.value)} placeholder="URL" type="text" value={url} />
                  )}
                />
                <AddButton
                  disabled={!isSaveEnabled}
                  iconFilled
                  label={videoStream ? 'Edit Stream' : 'Add Stream'}
                  onClick={() => {
                    videoStream
                      ? onEdit?.({
                          id: videoStream.id,
                          tvg_ID: epgId,
                          tvg_chno: channelNumber,
                          tvg_group: channelGroup,
                          tvg_logo: iconSource,
                          tvg_name: name,
                          url
                        } as UpdateVideoStreamRequest)
                      : onSave?.({
                          childVideoStreams: videoStream === undefined ? dataSource : videoStreams,
                          tvg_ID: epgId,
                          tvg_chno: channelNumber,
                          tvg_group: channelGroup,
                          tvg_logo: iconSource,
                          tvg_name: name,
                          url
                        } as CreateVideoStreamRequest);

                    setDataSource(undefined);
                  }}
                />
              </div>
            </div>
          </div>
        </div>
      </AccordionTab>
      <AccordionTab className="w-full h-full" header="Additional Streams">
        <div className="grid flex justify-content-start align-items-center surface-overlay m-0">
          <div className="flex col-12 p-0 justify-content-start align-items-center">
            <div className="col-6 m-0 p-0 pr-1">
              <VideoStreamDataSelector
                id="videostreampanel"
                onRowClick={(e) => {
                  if (videoStream?.id !== undefined || e === undefined || (dataSource !== undefined && dataSource?.findIndex((x) => x.id === e.id) !== -1)) {
                    return;
                  }

                  const ds = [...(dataSource ?? [])];

                  ds.push(e);
                  setDataSource(ds);
                  // console.log(e);
                }}
                videoStreamId={videoStream?.id}
              />
            </div>
            <div className="col-6 m-0 p-0 pr-1">
              <VideoStreamSelectedVideoStreamDataSelector
                dataSource={dataSource}
                id="videostreampanel"
                onRemove={(e) => {
                  if (videoStream?.id !== undefined || e === undefined) {
                    return;
                  }

                  if (dataSource?.findIndex((x) => x.id === e.id) !== -1) {
                    let ds = [...(dataSource ?? [])];
                    ds = ds.filter((x) => x.id !== e.id);
                    setDataSource(ds);
                  }
                }}
                OnRowReorder={(e) => {
                  setDataSource(e);
                }}
                videoStreamId={videoStream?.id}
              />
            </div>
          </div>
        </div>
        <div className="flex col-12 m-0 p-0 justify-content-end align-items-center w-full pt-2">
          <div className="flex col-6 m-0 p-0 align-items-center">
            <div className="flex col-6 m-0 p-0 justify-content-end align-items-center mr-2">Handler</div>
            <div className="flex col-6 m-0 p-0 align-items-center border-2 border-round surface-border ">
              <ChannelHandlerSelector className="w-full p-0 m-0" onChange={(e) => setChannelHandler(e)} value={channelHandler} />
            </div>
          </div>
        </div>
      </AccordionTab>
    </Accordion>
  );
};

VideoStreamPanel.displayName = 'Channel Panel';

export default memo(VideoStreamPanel);

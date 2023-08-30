
import { Accordion, AccordionTab } from "primereact/accordion";

import { InputNumber } from "primereact/inputnumber";
import { InputText } from "primereact/inputtext";

import { useState, useEffect, useMemo, memo } from "react";
import { getIconUrl } from "../../common/common";
import { type VideoStreamDto, type VideoStreamHandlers, type UpdateVideoStreamRequest, type CreateVideoStreamRequest } from "../../store/iptvApi";
import StreamMasterSetting from "../../store/signlar/StreamMasterSetting";
import ChannelHandlerSelector from "../../components/ChannelHandlerSelector";

import ChannelGroupSelector from "../../components/channelGroups/ChannelGroupSelector";
import EPGSelector from "../../components/selectors/EPGSelector";
import IconSelector from "../../components/selectors/IconSelector";
import { Button } from "primereact/button";
import VideoStreamDataSelector from "./VideoStreamDataSelector";
import VideoStreamSelectedVideoStreamDataSelector from "./VideoStreamSelectedVideoStreamDataSelector";

const VideoStreamPanel = (props: VideoStreamPanelProps) => {
  const settings = StreamMasterSetting();
  const [name, setName] = useState<string>('');
  const [url, setUrl] = useState<string>('');

  const [iconSource, setIconSource] = useState<string>('');

  const [videoStreams, setVideoStreams] = useState<VideoStreamDto[] | undefined>(undefined);

  const [channelNumber, setChannelNumber] = useState<number>(0);
  const [channelHandler, setChannelHandler] = useState<VideoStreamHandlers>(0);
  const [epgId, setEpgId] = useState<string>('');

  const [activeIndex, setActiveIndex] = useState<number>(0);
  const [lastActiveIndex, setLastActiveIndex] = useState<number>(0);


  const [channelGroup, setChannelGroup] = useState<string | undefined>(undefined);

  const [programme, setProgramme] = useState<string>('');

  useEffect(() => {

    if (props.group) {
      setChannelGroup(props.group);
    }
  }, [props.group]);


  useEffect(() => {
    if (props.videoStream === undefined) {
      return;
    }

    if (props.videoStream.childVideoStreams) {
      setVideoStreams(props.videoStream.childVideoStreams);
    }

    if (props.videoStream.user_Tvg_name && props.videoStream.user_Tvg_name !== undefined) {
      setName(props.videoStream.user_Tvg_name);
    }

    if (props.videoStream.user_Url && props.videoStream.user_Url !== undefined) {
      setUrl(props.videoStream.user_Url);
    }

    if (props.videoStream.user_Tvg_logo && props.videoStream.user_Tvg_logo !== undefined) {

      setIconSource(props.videoStream?.user_Tvg_logo);
    }

    if (props.videoStream.user_Tvg_chno && props.videoStream.user_Tvg_chno !== undefined) {
      setChannelNumber(props.videoStream.user_Tvg_chno);
    }

    if (props.videoStream.user_Tvg_ID && props.videoStream.user_Tvg_ID !== undefined) {
      setEpgId(props.videoStream.user_Tvg_ID);
    }

    if (props.videoStream.videoStreamHandler) {
      setChannelHandler(props.videoStream.videoStreamHandler);
    }

    if (props.videoStream.user_Tvg_group && props.videoStream.user_Tvg_group !== undefined && props.videoStream) {

      setChannelGroup(props.videoStream?.user_Tvg_group);
    }

  }, [props.videoStream]);

  const onIconChange = (newIconSource: string) => {
    if (!newIconSource) {
      return;
    }

    // setIcon(newIconSource);
    setIconSource(newIconSource);
  };

  const onEPGChange = (e: string) => {
    setProgramme(e);

    if (!e) {
      setEpgId('');
    } else {
      setEpgId(e);
    }
  };

  const isSaveEnabled = useMemo((): boolean => {
    if (!props.videoStream) {
      return name !== '';
    }

    if (props.videoStream.user_Tvg_name !== name) {
      return true;
    }

    if (props.videoStream.user_Tvg_chno !== channelNumber) {
      return true;
    }

    if (props.videoStream.user_Tvg_logo !== iconSource) {
      return true;
    }

    if (props.videoStream.user_Url !== url) {
      return true;
    }

    return false;

  }, [channelNumber, iconSource, name, props.videoStream, url]);

  // const ReturnToParent = () => {
  //   setChannelGroup(undefined);
  //   setName('');
  //   setUrl('');
  //   setIconSource('');
  //   setChannelNumber(0);
  //   setEpgId('');
  //   //  setVideoStreams([] as VideoStreamDto[]);
  //   props.onClose?.();
  // }





  const onsetActiveIndex = (index: number) => {

    if (index === null) {
      if (lastActiveIndex === 0) {
        index = 1;

      } else {
        index = 0;
      }
    }

    setLastActiveIndex(index);
    setActiveIndex(index);
  }

  return (
    <Accordion activeIndex={activeIndex} onTabChange={(e) => onsetActiveIndex(e.index as number)}>

      <AccordionTab header="General">
        <div className="grid flex flex-wrap justify-content-start align-items-center surface-overlay m-0">
          <div className="flex col-12 flex-wrap justify-content-start align-items-center p-0 m-0">

            {/* Image */}
            <div className='flex col-2 justify-content-center align-items-center'>
              <img
                alt={iconSource ?? 'Logo'}
                className="max-h-8rem h-8rem max-w-full"
                src={getIconUrl(iconSource, settings.defaultIcon, settings.cacheIcon)}
                style={{
                  objectFit: 'contain',
                }}
              />
            </div>

            <div className="flex flex-wrap col-10 justify-content-start align-items-center p-0 m-0 pl-4">
              <div className="flex flex-wrap col-9 justify-content-start align-items-center p-0 m-0">
                {/* Name */}
                <div className="flex col-12 p-0 m-0 pl-4 text-xs">
                  Name
                </div>
                <div className='flex col-12 justify-content-start align-items-center p-0 m-0'>
                  <InputText
                    autoFocus
                    className="withpadding p-inputtext-sm w-full"
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Name"
                    type="text"
                    value={name}
                  />
                </div>
              </div>

              <div className="flex flex-wrap col-3 justify-content-start align-items-center p-0 m-0">
                {/* Ch #*/}
                <div className="flex flex-wrap col-12 p-0 m-0 text-xs">
                  <div className="flex col-12 justify-content-center align-items-center p-0 m-0">
                    <span className="text-xs"                >
                      Ch. #
                    </span>
                  </div>
                  <div className='flex col-12 justify-content-center align-items-center p-0 m-0'>
                    <InputNumber
                      className='withpadding p-0 m-0'
                      id="channelNumber"
                      max={999999}
                      min={0}
                      onChange={(e) => { setChannelNumber(e.value ?? 0) }}
                      showButtons
                      size={3}
                      value={channelNumber}
                    />
                  </div>
                </div>
              </div>

              <div className="flex col-12 flex-wrap align-items-center justify-content-start p-0 m-0 mt-2 gap-2">
                {/* Logo Selector */}
                <div className="flex flex-wrap col-5 justify-content-start align-items-center p-0 m-0">
                  <div className="flex col-12 justify-content-start align-items-center p-0 m-0 pl-4 text-xs">
                    Logo
                  </div>
                  <div className='flex col-12 justify-content-start align-items-center p-0 m-0 border-2 border-round surface-border'>
                    <IconSelector
                      className="p-inputtext-sm"
                      onChange={(e) => onIconChange(e)}
                      value={iconSource}
                    />
                  </div>
                </div>

                {/* URL */}
                <div className="flex flex-wrap col-6 justify-content-start align-items-center p-0 m-0">

                  <div className="flex col-12 p-0 m-0 pl-4 text-xs">
                    Stream URL
                  </div>
                  <div className='flex col-12 justify-content-start align-items-center p-0 m-0'>
                    <InputText
                      className="withpadding p-inputtext-sm w-full"
                      onChange={(e) => setUrl(e.target.value)}
                      placeholder="URL"
                      type="text"
                      value={url}
                    />
                  </div>

                </div>

              </div>

            </div>
          </div>

          <div className='flex col-12 flex-wrap justify-content-start align-items-center p-0 m-0' >
            {/* EPG */}
            <div className="flex flex-wrap col-6 align-items-center justify-content-center p-0 m-0">
              <div className="flex col-12 justify-content-start align-items-center p-0 m-0 pl-4">
                <div className="text-xs"            >
                  EPG
                </div>
              </div>
              <div className='flex col-12 justify-content-start align-items-center p-0 pl-2 m-0 h-full border-2 border-round surface-border '>

                <EPGSelector
                  onChange={(e: string) => onEPGChange(e)}
                  value={programme}
                />

              </div>
            </div>

            {/* Group */}
            <div className="flex flex-wrap col-6 align-items-center justify-content-center p-0 m-0">
              <div className="flex col-12 justify-content-start align-items-center p-0 m-0 pl-4">
                <div
                  className="text-xs"
                >
                  Group
                </div>
              </div>
              <div className='flex col-10 justify-content-start align-items-center h-full p-0 pl-2 m-0 border-2 border-round surface-border'>
                <ChannelGroupSelector
                  onChange={(e) => setChannelGroup(e)}
                  value={channelGroup}
                />
              </div>
            </div>
          </div>

          <div className="flex col-12 align-items-center justify-content-end gap-1 p-0 mt-2">

            <Button
              disabled={!isSaveEnabled}
              icon="pi pi-check"
              label={props.videoStream ? "Edit" : "Add"}
              onClick={() => props.videoStream ? props.onEdit?.({
                id: props.videoStream.id,
                tvg_chno: channelNumber,
                tvg_group: channelGroup,
                tvg_ID: epgId,
                tvg_logo: iconSource,
                tvg_name: name,
                url: url
              } as UpdateVideoStreamRequest) : props.onSave?.({
                childVideoStreams: videoStreams,
                tvg_chno: channelNumber,
                tvg_group: channelGroup,
                tvg_ID: epgId,
                tvg_logo: iconSource,
                tvg_name: name,
                url: url
              } as CreateVideoStreamRequest
              )}
              rounded
              severity="success"
            />
          </div>
        </div >
      </AccordionTab>

      <AccordionTab header='Additional Streams'>
        <div className='flex col-12 flex-wrap justify-content-start align-items-center p-0 m-0 mt-2 w-full'>
          <div className='flex col-12 p-0 justify-content-start align-items-center w-full'>
            <div className='col-6 m-0 p-0 pr-1' >
              <VideoStreamDataSelector
                id='videostreampanel'
                videoStreamId={props.videoStream?.id ?? '00125dc876be8687dd57a1fd3405df44'}
              />
            </div>
            <div className='col-6 m-0 p-0 pr-1' >
              <VideoStreamSelectedVideoStreamDataSelector
                id='videostreampanel'
                videoStreamId={props.videoStream?.id ?? '00125dc876be8687dd57a1fd3405df44'}
              />
            </div>
          </div>
        </div>
        <div className='flex col-12 m-0 p-0 justify-content-end align-items-center w-full'>

          <div className='flex col-6 m-0 p-0 align-items-center'>
            <div className='flex col-6 m-0 p-0 justify-content-end align-items-center mr-2'>
              Handler
            </div>
            <div className='flex col-6 m-0 p-0 align-items-center border-2 border-round surface-border '>
              <ChannelHandlerSelector
                className="w-full p-0 m-0"
                onChange={(e) => setChannelHandler(e)}
                value={channelHandler}
              />
            </div>
          </div>

        </div>

      </AccordionTab>

    </Accordion>

  );
};

VideoStreamPanel.displayName = 'Channel Panel';
VideoStreamPanel.defaultProps = {

};
type VideoStreamPanelProps = {
  group?: string;
  onEdit?: (e: UpdateVideoStreamRequest) => void;
  onSave?: (e: CreateVideoStreamRequest) => void;
  videoStream?: VideoStreamDto | undefined;
};

export default memo(VideoStreamPanel);

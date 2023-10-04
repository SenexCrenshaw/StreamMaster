import StreamMasterSetting from '@/lib/StreamMasterSetting'
import { getIconUrl } from '@/lib/common/common'
import {
  CreateVideoStreamRequest,
  UpdateVideoStreamRequest,
  VideoStreamDto,
  VideoStreamHandlers,
} from '@/lib/iptvApi'
import ChannelHandlerSelector from '@/src/components/ChannelHandlerSelector'
import AddButton from '@/src/components/buttons/AddButton'
import ChannelGroupSelector from '@/src/components/channelGroups/ChannelGroupSelector'
import EPGSelector from '@/src/components/selectors/EPGSelector'
import IconSelector from '@/src/components/selectors/IconSelector'
import { Accordion, AccordionTab } from 'primereact/accordion'
import { InputNumber } from 'primereact/inputnumber'
import { InputText } from 'primereact/inputtext'
import { memo, useEffect, useMemo, useState } from 'react'
import InputWrapper from './InputWrapper'
import VideoStreamDataSelector from './VideoStreamDataSelector'
import VideoStreamSelectedVideoStreamDataSelector from './VideoStreamSelectedVideoStreamDataSelector'

const VideoStreamPanel = (props: VideoStreamPanelProps) => {
  const settings = StreamMasterSetting()
  const [name, setName] = useState<string>('')
  const [url, setUrl] = useState<string>('')

  const [iconSource, setIconSource] = useState<string>('')
  const [videoStreams, setVideoStreams] = useState<
    VideoStreamDto[] | undefined
  >(undefined)
  const [channelNumber, setChannelNumber] = useState<number>(0)
  const [channelHandler, setChannelHandler] = useState<VideoStreamHandlers>(0)
  const [epgId, setEpgId] = useState<string>('')
  const [activeIndex, setActiveIndex] = useState<number>(0)
  const [lastActiveIndex, setLastActiveIndex] = useState<number>(0)
  const [channelGroup, setChannelGroup] = useState<string | undefined>(
    undefined,
  )

  useEffect(() => {
    if (props.group) {
      setChannelGroup(props.group)
    }
  }, [props.group])

  useEffect(() => {
    const {
      childVideoStreams,
      user_Tvg_name: userTvgName,
      user_Url: userUrl,
      user_Tvg_logo: userTvgLogo,
      user_Tvg_chno: userTvgChno,
      user_Tvg_ID: userTvgId,
      videoStreamHandler,
      user_Tvg_group: userTvgGroup,
    } = props.videoStream ?? {}

    if (childVideoStreams) {
      setVideoStreams(childVideoStreams)
    }

    if (userTvgName) {
      setName(userTvgName)
    }

    if (userUrl) {
      setUrl(userUrl)
    }

    if (userTvgLogo) {
      setIconSource(userTvgLogo)
    }

    if (userTvgChno) {
      setChannelNumber(userTvgChno)
    }

    if (userTvgId) {
      setEpgId(userTvgId)
    }

    if (videoStreamHandler) {
      setChannelHandler(videoStreamHandler)
    }

    if (userTvgGroup) {
      setChannelGroup(userTvgGroup)
    }
  }, [props.videoStream])

  const isSaveEnabled = useMemo((): boolean => {
    if (!props.videoStream) {
      return name !== ''
    }

    if (props.videoStream.user_Tvg_name !== name) {
      return true
    }

    if (props.videoStream.user_Tvg_chno !== channelNumber) {
      return true
    }

    if (props.videoStream.user_Tvg_logo !== iconSource) {
      return true
    }

    if (props.videoStream.user_Url !== url) {
      return true
    }

    return false
  }, [channelNumber, iconSource, name, props.videoStream, url])

  const onsetActiveIndex = (index: number) => {
    if (index === null) {
      if (lastActiveIndex === 0) {
        index = 1
      } else {
        index = 0
      }
    }

    setLastActiveIndex(index)
    setActiveIndex(index)
  }

  return (
    <Accordion
      activeIndex={activeIndex}
      onTabChange={(e) => onsetActiveIndex(e.index as number)}
    >
      <AccordionTab header="General">
        <div className="grid flex justify-content-start align-items-center surface-overlay m-0">
          <div className="flex col-12 justify-content-start align-items-center p-0 m-0">
            {/* Image */}
            <div className="flex col-2 justify-content-center align-items-center">
              <img
                alt={iconSource ?? 'Logo'}
                className="icon-template-large"
                src={getIconUrl(
                  iconSource,
                  settings.defaultIcon,
                  settings.cacheIcon,
                )}
              />
            </div>

            <div className="flex flex-wrap col-10  left_border justify-content-between">
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
                  renderInput={() => (
                    <IconSelector
                      className="w-full bordered-text mr-2"
                      onChange={setIconSource}
                      value={iconSource}
                    />
                  )}
                />

                <InputWrapper
                  columnSize={2}
                  label="Ch. #"
                  renderInput={() => (
                    <InputNumber
                      className="w-full"
                      id="channelNumber"
                      max={999999}
                      min={0}
                      onChange={(e) => {
                        setChannelNumber(e.value ?? 0)
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
                  renderInput={() => (
                    <EPGSelector
                      className="w-full bordered-text mr-2"
                      onChange={setEpgId}
                      value={epgId}
                    />
                  )}
                />

                <InputWrapper
                  columnSize={6}
                  label="Group"
                  renderInput={() => (
                    <ChannelGroupSelector
                      className="w-full bordered-text"
                      onChange={(e) => setChannelGroup(e)}
                      value={channelGroup}
                    />
                  )}
                />
              </div>

              <div className="flex col-12 justify-content-between align-items-center">
                <InputWrapper
                  columnSize={11}
                  label="Stream URL"
                  renderInput={() => (
                    <InputText
                      className="w-full bordered-text"
                      onChange={(e) => setUrl(e.target.value)}
                      placeholder="URL"
                      type="text"
                      value={url}
                    />
                  )}
                />
                <AddButton
                  disabled={!isSaveEnabled}
                  iconFilled
                  label={props.videoStream ? 'Edit Stream' : 'Add Stream'}
                  onClick={() =>
                    props.videoStream
                      ? props.onEdit?.({
                          id: props.videoStream.id,
                          tvg_chno: channelNumber,
                          tvg_group: channelGroup,
                          tvg_ID: epgId,
                          tvg_logo: iconSource,
                          tvg_name: name,
                          url: url,
                        } as UpdateVideoStreamRequest)
                      : props.onSave?.({
                          childVideoStreams: videoStreams,
                          tvg_chno: channelNumber,
                          tvg_group: channelGroup,
                          tvg_ID: epgId,
                          tvg_logo: iconSource,
                          tvg_name: name,
                          url: url,
                        } as CreateVideoStreamRequest)
                  }
                />
              </div>
            </div>
          </div>
        </div>
      </AccordionTab>
      <AccordionTab
        disabled={!props.videoStream?.id}
        header="Additional Streams"
      >
        <div className="grid flex justify-content-start align-items-center surface-overlay m-0">
          <div className="flex col-12 p-0 justify-content-start align-items-center w-full ">
            <div className="col-6 m-0 p-0 pr-1">
              <VideoStreamDataSelector
                id="videostreampanel"
                videoStreamId={props.videoStream?.id}
              />
            </div>
            <div className="col-6 m-0 p-0 pr-1">
              <VideoStreamSelectedVideoStreamDataSelector
                id="videostreampanel"
                videoStreamId={props.videoStream?.id}
              />
            </div>
          </div>
        </div>
        <div className="flex col-12 m-0 p-0 justify-content-end align-items-center w-full pt-2">
          <div className="flex col-6 m-0 p-0 align-items-center">
            <div className="flex col-6 m-0 p-0 justify-content-end align-items-center mr-2">
              Handler
            </div>
            <div className="flex col-6 m-0 p-0 align-items-center border-2 border-round surface-border ">
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
  )
}

VideoStreamPanel.displayName = 'Channel Panel'

type VideoStreamPanelProps = {
  readonly group?: string
  readonly onEdit?: (e: UpdateVideoStreamRequest) => void
  readonly onSave?: (e: CreateVideoStreamRequest) => void
  readonly videoStream?: VideoStreamDto | undefined
}

export default memo(VideoStreamPanel)

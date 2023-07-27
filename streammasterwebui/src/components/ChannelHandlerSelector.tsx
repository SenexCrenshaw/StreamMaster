/* eslint-disable @typescript-eslint/consistent-type-imports */
/* eslint-disable @typescript-eslint/no-unused-vars */
import { type DropdownProps } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import * as React from 'react';
import * as StreamMasterApi from '../store/iptvApi';
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import { SelectItem } from 'primereact/selectitem';
import { VideoStreamHandlers } from '../store/streammaster_enums';

const ChannelHandlerSelector = (props: ChannelHandlerSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [channelHandler, setChannelHandler] = React.useState<StreamMasterApi.VideoStreamHandlers>(0);

  React.useMemo(() => {

    if (props.value && props.value !== undefined) {
      setChannelHandler(props.value);
    } else {
      setChannelHandler(0);
    }

  }, [props.value]);

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  const handlerOptionTemplate = React.useCallback((programmeName: StreamMasterApi.ProgrammeNameDto) => {
    return (
      <span className="text-sm" >{programmeName.displayName !== '' ? programmeName.displayName : programmeName.channelName}</span>
    );
  }, []);

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const onHandlerChange = React.useCallback(async (channel: any) => {

    if (props.onChange) {
      props.onChange(channel);
    }

  }, [props]);


  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(VideoStreamHandlers)
      .splice(0, Object.keys(VideoStreamHandlers).length / 2)
      .map(([number, word]) => {
        return {
          label: word,
          value: number,
        } as SelectItem;
      });

    return test;
  };

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const selectedTemplate = React.useCallback((option: any, propss: DropdownProps) => {

    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0 pl-2'>
        {channelHandler}
      </div>
    );

  }, [channelHandler]);

  if (props.enableEditMode !== true) {

    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {channelHandler}
      </div>
    )

  }


  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="iconSelector flex w-full justify-content-center align-items-center">
        <Dropdown
          className={className}
          onChange={async (e) => { await onHandlerChange(e.value); }}
          options={getHandlersOptions()}
          placeholder="Handler"
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
          value={channelHandler.toString()}
          virtualScrollerOptions={{
            itemSize: 32,
          }}
        />
      </div>
    </>
  );
}


ChannelHandlerSelector.displayName = 'ChannelHandlerSelector';
ChannelHandlerSelector.defaultProps = {
  className: null,
  enableEditMode: true,
  onChange: null,
  value: null,
};

type ChannelHandlerSelectorProps = {
  className?: string | null;
  enableEditMode?: boolean;
  onChange?: ((value: StreamMasterApi.VideoStreamHandlers) => void) | null;
  value?: StreamMasterApi.VideoStreamHandlers | null;
};

export default React.memo(ChannelHandlerSelector);


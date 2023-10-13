import { VideoStreamHandlers } from '@lib/common/streammaster_enums';
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import * as React from 'react';

type ChannelHandlerSelectorProps = {
  readonly className?: string | null;
  readonly onChange?: ((value: VideoStreamHandlers) => void) | null;
  readonly value?: VideoStreamHandlers | null;
};

const ChannelHandlerSelector = ({ className: propClassName, onChange, value }: ChannelHandlerSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [channelHandler, setChannelHandler] = React.useState<VideoStreamHandlers>(0);

  React.useMemo(() => {
    if (value && value !== undefined) {
      setChannelHandler(value);
    } else {
      setChannelHandler(0);
    }
  }, [value]);

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', propClassName);

  const onHandlerChange = React.useCallback(
    async (channel: any) => {
      if (onChange) {
        onChange(channel);
      }
    },
    [onChange],
  );

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

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="iconSelector flex w-full justify-content-center align-items-center">
        <Dropdown
          className={className}
          onChange={async (e) => {
            await onHandlerChange(e.value);
          }}
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
};

ChannelHandlerSelector.displayName = 'ChannelHandlerSelector';

export default React.memo(ChannelHandlerSelector);

import { M3UFileStreamUrlPrefix } from '@/lib/common/streammaster_enums';
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import * as React from 'react';

type StreamURLPrefixSelectorProps = {
  readonly className?: string | null;
  readonly onChange?: ((value: M3UFileStreamUrlPrefix) => void) | null;
  readonly value: M3UFileStreamUrlPrefix | null;
};

const StreamURLPrefixSelector = ({ className: propClassName, onChange, value }: StreamURLPrefixSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [streamURLPrefix, setStreamURLPrefix] = React.useState<M3UFileStreamUrlPrefix>(0);

  React.useMemo(() => {
    if (value && value !== undefined) {
      setStreamURLPrefix(value);
    } else {
      setStreamURLPrefix(0);
    }
  }, [value]);

  const className = classNames('p-0 m-0 w-full z-5 ', propClassName);

  const onHandlerChange = React.useCallback(
    async (channel: any) => {
      if (onChange) {
        onChange(channel);
      }
    },
    [onChange],
  );

  const getWord = (word: string): string => {
    if (word === 'SystemDefault') {
      word = 'Default';
    } else if (word === 'TS') {
      word = 'MPEG-TS(.ts)';
    } else if (word === 'M3U8') {
      word = 'HLS(.m3u8)';
    }
    return word;
  };

  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(M3UFileStreamUrlPrefix)
      .splice(0, Object.keys(M3UFileStreamUrlPrefix).length / 2)
      .map(([number, word]) => {
        return {
          label: getWord(word as string),
          value: number,
        } as SelectItem;
      });

    return test;
  };

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="flex w-full justify-content-center align-items-center">
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
          value={streamURLPrefix.toString()}
          virtualScrollerOptions={{
            itemSize: 32,
          }}
        />
      </div>
    </>
  );
};

StreamURLPrefixSelector.displayName = 'StreamURLPrefixSelector';

export default React.memo(StreamURLPrefixSelector);

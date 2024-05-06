import { M3UFileStreamURLPrefix } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import { memo, useCallback, useEffect, useRef, useState } from 'react';

interface StreamURLPrefixSelectorProperties {
  readonly className?: string | null;
  readonly onChange?: ((value: M3UFileStreamURLPrefix) => void) | null;
  readonly value: M3UFileStreamURLPrefix | null;
}

const StreamURLPrefixSelector = ({ className: propertyClassName, onChange, value }: StreamURLPrefixSelectorProperties) => {
  const toast = useRef<Toast>(null);
  const [streamURLPrefix, setStreamURLPrefix] = useState<M3UFileStreamURLPrefix>(0);

  useEffect(() => {
    if (value && value !== undefined) {
      setStreamURLPrefix(value);
    } else {
      setStreamURLPrefix(0);
    }
  }, [value]);

  const className = classNames('p-0 m-0 w-full z-5 ', propertyClassName);

  const onHandlerChange = useCallback(
    async (channel: any) => {
      if (onChange) {
        onChange(channel);
      }
    },
    [onChange]
  );

  const getWord = (word: string): string => {
    switch (word) {
      case 'SystemDefault': {
        word = 'Default';

        break;
      }
      case 'TS': {
        word = 'MPEG-TS(.ts)';

        break;
      }
      case 'M3U8': {
        word = 'HLS(.m3u8)';

        break;
      }
      // No default
    }
    return word;
  };

  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(M3UFileStreamURLPrefix)
      .splice(0, Object.keys(M3UFileStreamURLPrefix).length / 2)
      .map(
        ([number, word]) =>
          ({
            label: getWord(word as string),
            value: number
          } as SelectItem)
      );

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
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap'
          }}
          value={streamURLPrefix.toString()}
          virtualScrollerOptions={{
            itemSize: 32
          }}
        />
      </div>
    </>
  );
};

StreamURLPrefixSelector.displayName = 'StreamURLPrefixSelector';

export default memo(StreamURLPrefixSelector);

import React from "react";

import { Dropdown } from "primereact/dropdown";
import { classNames } from "primereact/utils";
import { type TooltipOptions } from "primereact/tooltip/tooltipoptions";
import { useDebouncedCallback } from "use-debounce";
import { useClickOutside } from "primereact/hooks";
import { BlockUI } from "primereact/blockui";

const DropDownEditorBodyTemplate = (props: DropDownEditorBodyTemplateProps) => {
  const [originalValue, setOriginalValue] = React.useState<string>('');
  const [isFocused, setIsFocused] = React.useState<boolean>(false);

  const overlayRef = React.useRef(null);
  const [inputValue, setInputValue] = React.useState<string>('');
  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  const debounced = useDebouncedCallback(
    React.useCallback((value) => {

      if (value !== originalValue) {
        setInputValue(value);

        props.onChange(value);
      }
    }, [originalValue, props]),
    250,
    {}
  );

  const save = React.useCallback((forceValueSave?: string | undefined) => {

    if (forceValueSave === undefined && (
      inputValue === undefined || inputValue === originalValue)
    ) {
      return;
    }

    debounced.cancel();

    if (forceValueSave !== undefined) {
      props.onChange(forceValueSave);
    } else {
      props.onChange(inputValue);
    }

  }, [debounced, inputValue, originalValue, props]);

  // Keyboard Enter
  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (!isFocused) {
        return;
      }

      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        save();
      }
    };

    document.addEventListener('keydown', callback);

    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [isFocused, save]);

  useClickOutside(overlayRef, () => {
    if (!isFocused) {
      return;
    }

    setIsFocused(false);

    if (originalValue !== inputValue) {
      save();
    }
  });

  React.useEffect(() => {
    if (props.value !== null && props.value !== undefined) {
      setInputValue(props.value);
      setOriginalValue(props.value);
    }

  }, [props.value, setInputValue]);

  return (
    <BlockUI blocked={props.isLoading}>
      <div ref={overlayRef}>
        <Dropdown
          className={className}
          editable
          filter
          filterBy='channelName'

          onChange={
            (e) => {
              setInputValue(e.target.value as string);
              debounced(e.target.value as string);
            }
          }
          onFocus={() => setIsFocused(true)}
          options={props.data}
          placeholder="No EPG"
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
          tooltip={props.tooltip}
          tooltipOptions={props.tooltipOptions}
          value={inputValue}
          virtualScrollerOptions={{
            itemSize: 32,
          }}
        />
      </div>
    </BlockUI>
  );
}

DropDownEditorBodyTemplate.displayName = 'DropDownEditorBodyTemplate';
DropDownEditorBodyTemplate.defaultProps = {

};

type DropDownEditorBodyTemplateProps = {
  className?: string;
  data: string[];
  isLoading?: boolean;
  onChange: (value: string) => void;
  tooltip?: string;
  tooltipOptions?: TooltipOptions;
  value: string;
};

export default React.memo(DropDownEditorBodyTemplate);

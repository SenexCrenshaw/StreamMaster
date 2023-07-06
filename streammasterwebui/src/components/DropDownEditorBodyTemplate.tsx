
import React from "react";

import { Dropdown } from "primereact/dropdown";
import { classNames } from "primereact/utils";
import { type TooltipOptions } from "primereact/tooltip/tooltipoptions";
import { useDebouncedCallback } from "use-debounce";
import { useClickOutside } from "primereact/hooks";

const DropDownEditorBodyTemplate = (props: DropDownEditorBodyTemplateProps) => {
  const [originalValue, setOriginalValue] = React.useState<string>('');
  const [isFocused, setIsFocused] = React.useState<boolean>(false);

  const [ignoreSave, setIgnoreSave] = React.useState<boolean>(true);
  const overlayRef = React.useRef(null);

  const [inputValue, setInputValue] = React.useState<string>('');

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);


  const debounced = useDebouncedCallback(
    React.useCallback((value) => {

      if (!ignoreSave && value !== originalValue) {
        setInputValue(value);
        setIgnoreSave(true);
        // console.log('Saved', value);
        props.onChange(value);
      }
    }, [ignoreSave, originalValue, props]),
    1500,
    {}
  );

  const save = React.useCallback((forceValueSave?: string | undefined) => {

    if (forceValueSave === undefined && (
      ignoreSave || inputValue === undefined || inputValue === originalValue)
    ) {
      return;
    }

    debounced.cancel();
    setIgnoreSave(true);
    if (forceValueSave !== undefined) {
      props.onChange(forceValueSave);
    } else {
      props.onChange(inputValue);
    }


  }, [debounced, ignoreSave, inputValue, originalValue, props]);


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
    setIsFocused(false);
    if (originalValue !== inputValue) {
      // console.log('useClickOutside saved');
      save();
    }
  });


  React.useMemo(() => {

    // if (props.value !== undefined && props.value !== originalValue) {
    if (props.value !== undefined) {
      // console.log('setOriginalValue', props.value);
      setInputValue(props.value);
      setOriginalValue(props.value);
      setIgnoreSave(false);
    }

  }, [props.value, setInputValue]);

  return (
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
        onClick={
          () => {
            props.onClick?.();
          }
        }
        onFocus={() => setIsFocused(true)}
        options={props.data ?? ['Dummy']}
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
  );
}

DropDownEditorBodyTemplate.displayName = 'DropDownEditorBodyTemplate';
DropDownEditorBodyTemplate.defaultProps = {

};

type DropDownEditorBodyTemplateProps = {
  className?: string | null;
  data: string[] | null;
  onChange: (value: string) => void;
  onClick?: () => void;
  tooltip?: string | undefined;
  tooltipOptions?: TooltipOptions | undefined;
  value: string | undefined;
};

export default React.memo(DropDownEditorBodyTemplate);


import { useDebouncedCallback } from 'use-debounce';
import { InputText } from "primereact/inputtext";
import React from "react";
import { useClickOutside } from 'primereact/hooks';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { Button } from 'primereact/button';
import { getTopToolOptions } from '../common/common';
import { ResetLogoIcon } from '../common/icons';

const StringEditorBodyTemplate = (props: StringEditorBodyTemplateProps) => {
  const [originalValue, setOriginalValue] = React.useState<string>('');
  const [isFocused, setIsFocused] = React.useState<boolean>(false);

  const [ignoreSave, setIgnoreSave] = React.useState<boolean>(true);
  const overlayRef = React.useRef(null);

  const [inputValue, setInputValue] = React.useState<string>('');


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

    <div className='p-0 relative' ref={overlayRef}
      style={{
        ...{
          backgroundColor: 'var(--mask-bg)',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap',
        },
      }}
    >
      {(isFocused && props.resetValue !== undefined && props.resetValue !== inputValue) &&
        < Button
          className="absolute right-0"
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(props.resetValue !== undefined ? props.resetValue : '');
            save(props.resetValue);
          }
          }
          rounded
          severity="warning"
          size="small"
          text
          tooltip="Reset Name"
          tooltipOptions={getTopToolOptions}
        />
      }
      <InputText
        className="sm-inputtext p-0 flex justify-content-start w-full text-sm"
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

        tooltip={props.tooltip}
        tooltipOptions={props.tooltipOptions}
        value={inputValue}
      />
    </div>

  );
}

export type StringEditorBodyTemplateProps = {
  onChange: (value: string) => void;
  onClick?: () => void;
  // onReset?: ((value: string) => void);
  resetValue?: string | undefined;
  tooltip?: string | undefined;
  tooltipOptions?: TooltipOptions | undefined;
  value: string | undefined;
};

export default React.memo(StringEditorBodyTemplate);

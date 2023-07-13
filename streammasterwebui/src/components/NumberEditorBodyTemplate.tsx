
import { InputNumber } from "primereact/inputnumber";
import { type TooltipOptions } from "primereact/tooltip/tooltipoptions";
import { type CSSProperties } from "react";
import React from "react";
import { useDebouncedCallback } from "use-debounce";
import { getTopToolOptions } from "../common/common";
import { Button } from "primereact/button";
import { ResetLogoIcon } from "../common/icons";
import { useClickOutside } from "primereact/hooks";

const NumberEditorBodyTemplate = (props: NumberEditorBodyTemplateProps) => {
  const [inputValue, setInputValue] = React.useState<number>(0);
  const [originalValue, setOriginalValue] = React.useState<number>(0);
  const [ignoreSave, setIgnoreSave] = React.useState<boolean>(true);
  const [isFocused, setIsFocused] = React.useState<boolean>(false);
  const overlayRef = React.useRef(null);


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

  const save = React.useCallback((forceValueSave?: number | undefined) => {
    // console.log('save', ignoreSave, inputValue, originalValue)
    if (forceValueSave === undefined && (
      ignoreSave || inputValue === undefined || inputValue === originalValue)
    ) {
      return;
    }

    debounced.cancel();
    setIgnoreSave(true);
    // console.log('Saved', inputValue);
    if (forceValueSave !== undefined) {
      props.onChange(forceValueSave);
    } else {
      props.onChange(inputValue);
    }


  }, [debounced, ignoreSave, inputValue, originalValue, props]);



  useClickOutside(overlayRef, () => {
    if (!isFocused) {
      return;
    }

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
    <div className='relative' ref={overlayRef}
      style={props.style}
    >
      {(isFocused && props.resetValue !== undefined && props.resetValue !== 0 && props.resetValue !== inputValue) &&
        <Button
          className="absolute mt-1 right-0"
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            setInputValue(props.resetValue !== undefined ? props.resetValue : 0);
            // props?.onReset?.(props.resetValue !== undefined ? props.resetValue : '');
            save(props.resetValue);
          }
          }
          rounded
          severity="warning"
          size="small"
          tooltip="Reset ChNo"
          tooltipOptions={getTopToolOptions}
        />
      }
      <InputNumber
        className="text-sm w-full"
        locale="en-US"
        onChange={(e) => {
          debounced(e.value as number);
          setInputValue(e.value as number);
        }}
        onClick={() => { props.onClick?.(); }}
        onFocus={() => setIsFocused(true)}
        prefix={props.prefix}

        suffix={props.suffix}
        tooltip={props.tooltip}
        tooltipOptions={props.tooltipOptions}
        value={inputValue}
      />
    </div>
  );
}


export type NumberEditorBodyTemplateProps = {
  onChange: (value: number) => void;
  onClick?: () => void;
  // onReset?: ((value: string) => void);
  prefix?: string | undefined;
  resetValue?: number | undefined;
  style?: CSSProperties;
  suffix?: string | undefined;
  tooltip?: string | undefined;
  tooltipOptions?: TooltipOptions | undefined;
  value: number | undefined;
};

export default React.memo(NumberEditorBodyTemplate);

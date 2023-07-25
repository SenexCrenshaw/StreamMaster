
import { Button } from 'primereact/button';
import { type DropdownChangeEvent } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import * as React from 'react';
import { type IconFileDto } from '../store/iptvApi';
import { useIconsGetIconsQuery } from '../store/iptvApi';
import StreamMasterSetting from '../store/signlar/StreamMasterSetting';
import { getIconUrl, getTopToolOptions } from '../common/common';

import { ResetLogoIcon } from '../common/icons';

const IconSelector = (props: IconSelectorProps) => {
  const [selectedIcon, setSelectedIcon] = React.useState<string>('');
  const [resetIcon, setResetIcon] = React.useState<string>('');
  const setting = StreamMasterSetting();

  const icons = useIconsGetIconsQuery();

  React.useEffect(() => {
    if (props.value) {
      setSelectedIcon(props.value);
    }

    if (props.resetValue) {
      setResetIcon(props.resetValue);
    }

  }, [props.value, props.resetValue]);


  const onChange = React.useCallback((event: DropdownChangeEvent) => {
    setSelectedIcon(event.value);

    if (!event.value) return;

    const icon = event.value as string;

    props.onChange?.(icon);

  }, [props]);

  const selectedTemplate = React.useCallback((option: IconFileDto) => {
    if (!option || option.source === undefined || option.source === null || option.source === '') {
      return (<div />);
    }

    const iconUrl = getIconUrl(option.source, setting.defaultIcon, false);

    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0 pl-2'>

        <img
          alt={option.name}
          src={iconUrl}
          style={{
            maxWidth: '1.2rem',
            objectFit: 'contain',
            width: '1.2rem',
          }}
        />
      </div>
    );
  }, [setting]);

  const iconOptionTemplate = React.useCallback((option: IconFileDto) => {

    const iconUrl = getIconUrl(option.source, setting.defaultIcon, false);

    return (
      <>
        <img
          alt={option.name ?? 'name'}
          className="flex align-items-center max-w-full h-2rem text-base text-color surface-overlay appearance-none outline-none focus:border-primary"
          src={iconUrl}
        />
        <div className="white-space-normal">{option.name}</div>
      </>
    );
  }, [setting]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
  const filterTemplate = React.useCallback((options: any) => {
    return (
      <div className="flex gap-2 align-items-center">
        <div hidden={props.onAddIcon === undefined}>
          <Button
            icon="pi pi-plus"
            onClick={() => props.onAddIcon !== undefined ? props.onAddIcon() : null}
            rounded
            severity="success"
            size="small"
            tooltip="Add Icon File"
            tooltipOptions={getTopToolOptions}
          />
        </div>
        {options.element}
        {props.resetValue !== props.value &&
          <Button
            icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
            onClick={() => {
              if (resetIcon !== undefined) {
                props.onReset(resetIcon)
              } else if (setting.defaultIconDto !== undefined) {
                props.onReset(setting.defaultIconDto.source)
              }
            }
            }
            rounded
            severity="warning"
            size="small"
            style={{
              ...{
                maxHeight: "2rem",
                maxWidth: "2rem"
              }
            }}
            tooltip="Reset Logo"
            tooltipOptions={getTopToolOptions}
          />
        }
      </div>
    );
  }, [props, resetIcon, setting.defaultIconDto]);


  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className, {
    'p-button-loading': props.loading,
    'p-disabled': props.disabled,
  });


  if (props.enableEditMode !== true) {
    const iconUrl = getIconUrl(selectedIcon, setting.defaultIcon, false);
    return (
      <img
        alt='logo'
        className="max-h-1rem"
        onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
        src={iconUrl}
        style={{
          maxWidth: '1.5rem',
          objectFit: 'contain',
        }}
      />
    )
  }


  return (
    <div className="iconSelector flex w-full">
      <Dropdown
        className={className}
        disabled={props.disabled}
        filter
        filterBy="name"
        filterInputAutoFocus
        filterTemplate={filterTemplate}
        itemTemplate={iconOptionTemplate}
        onChange={onChange}
        optionLabel="name"
        optionValue="source"
        options={icons.data}
        placeholder="Select an Icon"
        style={{
          ...{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          },
        }}
        value={selectedIcon}
        valueTemplate={selectedTemplate}
        virtualScrollerOptions={{
          itemSize: 72,
        }}

      />
    </div>
  );
};

IconSelector.displayName = 'IconSelector';
IconSelector.defaultProps = {
  className: null,
  disabled: false,
  enableEditMode: true,
  loading: false
};

type IconSelectorProps = {
  className?: string | null;
  disabled?: boolean;
  enableEditMode?: boolean;
  loading?: boolean;
  onAddIcon?: () => void;
  onChange: ((value: string) => void);
  onReset: ((value: string) => void);
  resetValue: string;
  value: string;
};

export default React.memo(IconSelector);

import { Button } from 'primereact/button';
import { type DropdownChangeEvent } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import * as React from 'react';
import { type IconFileDto } from '../store/iptvApi';
import { useIconsGetIconsQuery } from '../store/iptvApi';
import StreamMasterSetting from '../store/signlar/StreamMasterSetting';
import FileDialog from './FileDialog';
import { getTopToolOptions } from '../common/common';

import { ResetLogoIcon } from '../common/icons';
import { LazyLoadImage } from 'react-lazy-load-image-component';
import { baseHostURL, isDev } from '../settings';

const IconSelector = (props: IconSelectorProps) => {


  const [selectedIcon, setSelectedIcon] = React.useState<IconFileDto>();
  const [resetIcon, setResetIcon] = React.useState<IconFileDto>();
  const setting = StreamMasterSetting();
  const icons = useIconsGetIconsQuery();


  React.useEffect(() => {
    if (!icons.data) {
      return;
    }

    if (props.value) {
      const tests = icons.data.filter((a: IconFileDto) => a.url === props.value);

      if (tests && tests !== undefined && tests.length > 0) {
        setSelectedIcon(tests[0]);
      }
    }

    if (props.resetValue) {
      const tests = icons.data.filter((a: IconFileDto) => a.originalSource === props.resetValue);

      if (tests && tests !== undefined && tests.length > 0) {
        setResetIcon(tests[0]);
      }
    }

  }, [icons, props.value, props.resetValue]);


  const onChange = React.useCallback((event: DropdownChangeEvent) => {
    setSelectedIcon(event.value);

    if (!event.value) return;

    const icon = event.value as IconFileDto;

    props.onChange?.(icon);

  }, [props]);

  const selectedTemplate = React.useCallback((option: IconFileDto) => {
    if (!option || option.url === undefined || option.url === null || option.url === '') {
      return (<div />);
      // <QuestionMarkIcon className='flex col-1 m-0 text-red-500' sx={{ fontSize: 32 }} />
      // );
    }

    let selectedTemplateurl = option.url ?? setting.defaultIcon;

    if (isDev && selectedTemplateurl && !selectedTemplateurl.startsWith('http')) {
      selectedTemplateurl = baseHostURL + selectedTemplateurl;
    }


    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0 pl-2'>
        <LazyLoadImage
          alt={option.name}
          loading="lazy"
          src={selectedTemplateurl}
          style={{
            maxWidth: '1.2rem',
            objectFit: 'contain',
          }}
        />
      </div>
    );
  }, [setting.defaultIcon]);

  const iconOptionTemplate = React.useCallback((option: IconFileDto) => {

    let iconOptionTemplateurl = option.url ?? setting.defaultIcon;

    if (isDev && iconOptionTemplateurl && !iconOptionTemplateurl.startsWith('http')) {
      iconOptionTemplateurl = baseHostURL + iconOptionTemplateurl;
    }

    return (
      <>
        <LazyLoadImage
          alt={option.name ?? 'name'}
          className="flex align-items-center max-w-full h-2rem text-base text-color surface-overlay appearance-none outline-none focus:border-primary"
          loading="lazy"

          src={iconOptionTemplateurl}
        />
        <div className="white-space-normal">{option.name}</div>
      </>
    );
  }, [setting.defaultIcon]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
  const filterTemplate = React.useCallback((options: any) => {
    return (
      <div className="flex gap-2 align-items-center">
        <FileDialog
          fileType="icon"
          onHide={() => { }}
        />

        {options.element}
        {/* {JSON.stringify(props.resetValue === props.value)}
        {JSON.stringify(props.resetValue)}
        {JSON.stringify(props.value)} */}
        {props.resetValue !== props.value && <Button
          icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
          onClick={() => {
            if (resetIcon !== undefined) {
              props.onReset(resetIcon)
            } else if (setting.defaultIconDto !== undefined) {
              props.onReset(setting.defaultIconDto)
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

  let url = selectedIcon?.url ?? setting.defaultIcon;
  if (isDev && url && !url.startsWith('http')) {
    url = baseHostURL + url;
  }

  if (props.enableEditMode !== true) {
    return (
      <img
        alt='logo'
        className="max-h-1rem"
        onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
        src={url}
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
        // filterInputAutoFocus
        filterTemplate={filterTemplate}
        itemTemplate={iconOptionTemplate}
        onChange={onChange}
        optionLabel="name"
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
  onChange: ((value: IconFileDto) => void);
  onReset: ((value: IconFileDto) => void);
  resetValue: string;
  value: string;
};

export default React.memo(IconSelector);

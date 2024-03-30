import AddButton from '@components/buttons/AddButton';
import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';
import { getIconUrl } from '@lib/common/common';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import { IconFileDto } from '@lib/smAPI/smapiTypes';

import useSettings from '@lib/useSettings';
import { Dropdown } from 'primereact/dropdown';
import { ProgressSpinner } from 'primereact/progressspinner';
import { classNames } from 'primereact/utils';
import React, { useEffect, useState } from 'react';

type IconSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  readonly useDefault?: boolean;
};

const IconSelector = ({ enableEditMode = true, value, disabled, editable = true, onChange, useDefault }: IconSelectorProperties) => {
  const setting = useSettings();
  const [input, setInput] = useState<string | undefined>(undefined);
  // const [oldInput, setOldInput] = useState<string | undefined>(undefined);
  // const [filterValue, setFilterValue] = useState<string | undefined>(undefined);
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [icon, setIcon] = useState<IconFileDto | undefined>(undefined);
  const query = useGetIcons();

  useEffect(() => {
    if (value && input === undefined) {
      console.log('setInput true');
      setInput(value);
    } else {
      console.log('setInput false');
    }
  }, [value, input]);

  useEffect(() => {
    if (checkValue === undefined && !query.isError && input !== undefined) {
      setCheckValue(input);
      const entry = query.data?.find((x) => x.source === input);
      if (entry && entry.source !== icon?.source) {
        setIcon(entry);
      } else {
        setIcon(undefined);
      }
    }
  }, [checkValue, icon?.source, input, query]);

  const selectedTemplate = (option: any) => {
    if (option === null) return <div />;

    const iconUrl = option?.source ? getIconUrl(option.source, setting.defaultIcon, false) : '';

    if (!iconUrl) {
      return <div />;
    }

    return (
      <div className="icon-template">
        <img alt="Icon logo" src={iconUrl} />
      </div>
    );
  };

  const handleOnChange = (source: string) => {
    if (!source) {
      return;
    }

    const entry = query.data?.find((x) => x.source === source);
    if (entry && entry.source !== icon?.source) {
      setIcon(entry);
    } else {
      setIcon(undefined);
    }

    setInput(source);
    onChange && onChange(source);
  };

  const panelTemplate = (option: any) => {
    return (
      <div className="iconselector flex flex-row grid col-12 m-0 p-0 justify-content-between align-items-center">
        <div className="col-12 m-0 p-0 flex flex-row">
          <div className="col-11 m-0 p-0 pl-2">
            <StringEditorBodyTemplate
              disableDebounce={true}
              placeholder="Custom URL"
              value={input}
              onChange={(value) => {
                if (value) {
                  setInput(value);
                }
              }}
            />
          </div>
          <div className="col-1 m-0 p-0">
            <AddButton
              tooltip="Add Custom URL"
              iconFilled={false}
              onClick={(e) => {
                if (input) {
                  handleOnChange(input);
                }
              }}
              style={{
                width: 'var(--input-height)',
                height: 'var(--input-height)'
              }}
            />
          </div>
        </div>
      </div>
    );
  };

  const itemTemplate = (option: IconFileDto): JSX.Element => {
    if (!option) {
      return <div />;
    }
    const iconUrl = getIconUrl(option.source ?? '', setting.defaultIcon, false);

    if (useDefault !== true && !iconUrl) return <div />;

    return (
      <div className="icon-option-template">
        <img alt={option?.name || 'name'} src={iconUrl} loading="lazy" />
        <div className="icon-option-name">{option?.name}</div>
      </div>
    );
  };

  if (!enableEditMode) {
    const iconUrl = getIconUrl(value ?? '', setting.defaultIconUrl, false);

    return <img alt="logo" className="default-icon" src={iconUrl} loading="lazy" />;
  }

  const className = classNames('align-contents-center p-0 m-0 max-w-full w-full iconselector', {
    'p-disabled': disabled
  });

  const loading = query.isError || query.isLoading || !query.data;

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0 iconselector">{input ?? 'Dummy'}</div>;
  }

  if (loading) {
    return (
      <div className="m-0 p-0">
        <ProgressSpinner />
      </div>
    );
  }

  return (
    <div className="flex align-contents-center w-full min-w-full iconselector">
      <Dropdown
        className={className}
        disabled={loading}
        filterClearIcon="pi pi-times"
        itemTemplate={itemTemplate}
        filter
        filterInputAutoFocus
        onChange={(e) => {
          handleOnChange(e?.value?.id);
        }}
        optionLabel="name"
        options={query.data}
        panelFooterTemplate={panelTemplate}
        placeholder="placeholder"
        // resetFilterOnHide
        showFilterClear
        value={icon}
        valueTemplate={selectedTemplate}
        virtualScrollerOptions={{
          itemSize: 42,
          style: {
            minWidth: '400px',
            width: '400px',
            maxWidth: '50vw'
          }
        }}
      />
    </div>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

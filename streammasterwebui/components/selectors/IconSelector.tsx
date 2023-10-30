import { getIconUrl } from '@lib/common/common';
import { IconFileDto, useIconsGetIconsSimpleQueryQuery, useIconsGetPagedIconsQuery } from '@lib/iptvApi';
import { GetIconFromSource } from '@lib/smAPI/Icons/IconsGetAPI';
import useSettings from '@lib/useSettings';
import React, { useCallback } from 'react';
import BaseSelector, { BaseSelectorProps as BaseSelectorProperties } from './BaseSelector';

type IconSelectorProperties = BaseSelectorProperties<IconFileDto> & {
  enableEditMode?: boolean;
  useDefault?: boolean;
};

const IconSelector: React.FC<Partial<IconSelectorProperties>> = ({ enableEditMode = true, useDefault, onChange, ...restProperties }) => {
  const setting = useSettings();

  const selectedTemplate = (option: any) => {
    const iconUrl = option?.source ? getIconUrl(restProperties.value, setting.defaultIcon, false) : '';

    if (!iconUrl) return <div />;

    return (
      <div className="icon-template">
        <img alt="Icon logo" src={iconUrl} />
      </div>
    );
  };

  const iconOptionTemplate = (option: IconFileDto): JSX.Element => {
    const iconUrl = getIconUrl(option.source ?? '', setting.defaultIcon, false);

    if (useDefault !== true && !iconUrl) return <div />;

    return (
      <div className="icon-option-template">
        <img alt={option?.name || 'name'} src={iconUrl} />
        <div className="icon-option-name">{option?.name}</div>
      </div>
    );
  };

  const handleOnChange = useCallback(
    (event: string) => {
      if (event && onChange) onChange(event);
    },
    [onChange]
  );

  if (!enableEditMode) {
    const iconUrl = getIconUrl(restProperties.value ?? '', setting.defaultIconUrl, false);

    return <img alt="logo" className="default-icon" src={iconUrl} />;
  }

  return (
    <BaseSelector
      {...restProperties}
      itemSize={64}
      itemTemplate={iconOptionTemplate}
      onChange={handleOnChange}
      optionLabel="name"
      optionValue="source"
      queryFilter={useIconsGetPagedIconsQuery}
      queryHook={useIconsGetIconsSimpleQueryQuery}
      querySelectedItem={GetIconFromSource}
      selectName="Icon"
      selectedTemplate={selectedTemplate}
    />
  );
};

IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

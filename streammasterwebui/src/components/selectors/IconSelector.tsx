import { type IconFileDto, useIconsGetIconsQuery } from '../../store/iptvApi';
import { useIconsGetIconsSimpleQueryQuery } from '../../store/iptvApi';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';
import React, { useCallback } from 'react';
import { GetIconFromSource } from '../../store/signlar_functions';
import BaseSelector, { type BaseSelectorProps } from './BaseSelector';

import { getIconUrl } from '../../common/common';

type IconSelectorProps = BaseSelectorProps<IconFileDto> & {
  enableEditMode?: boolean;
  useDefault?: boolean;
};

const IconSelector: React.FC<Partial<IconSelectorProps>> = ({
  enableEditMode = true,
  useDefault,
  onChange,
  ...restProps
}) => {

  const setting = StreamMasterSetting();

  const selectedTemplate = (option: IconFileDto) => {
    const iconUrl = option?.source ? getIconUrl(option.source, setting.defaultIcon, false) : '';

    if (!iconUrl) return <div />;

    return (
      <div className='icon-template'>
        <img alt='Icon logo' src={iconUrl} />
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
  }

  const handleOnChange = useCallback((event: string) => {
    if (event && onChange) onChange(event);
  }, [onChange]);

  if (!enableEditMode) {
    const iconUrl = getIconUrl(restProps.value ?? '', setting.defaultIconUrl, false);
    return (
      <img alt='logo' className="default-icon" src={iconUrl} />
    )
  }

  return (
    <BaseSelector
      {...restProps}
      itemSize={72}
      itemTemplate={iconOptionTemplate}
      onChange={handleOnChange}
      optionLabel="name"
      optionValue="source"
      queryFilter={useIconsGetIconsQuery}
      queryHook={useIconsGetIconsSimpleQueryQuery}
      querySelectedItem={GetIconFromSource}
      selectName='Icon'
      selectedTemplate={selectedTemplate}
    />
  );
};

IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

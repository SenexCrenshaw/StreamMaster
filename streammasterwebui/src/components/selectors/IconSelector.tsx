import { useIconsGetIconsQuery } from '../../store/iptvApi';
import { type IconFileDto } from '../../store/iptvApi';
import { useIconsGetIconsSimpleQueryQuery } from '../../store/iptvApi';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';
import React, { useState, useCallback } from 'react';
import { GetIconFromSource } from '../../store/signlar_functions';
import BaseSelector, { type BaseSelectorProps } from './BaseSelector';
import { type GetApiArg } from '../../common/common';
import { type SimpleQueryApiArg } from '../../common/common';
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

  const [paging, setPaging] = useState<SimpleQueryApiArg>({ first: 0, last: 40 });
  const [filter, setFilter] = useState<GetApiArg>({ pageSize: 40 });

  const filteredIconData = useIconsGetIconsQuery(filter);
  const data = useIconsGetIconsSimpleQueryQuery(paging);

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
      data={data.data ?? []}
      fetch={GetIconFromSource}
      filteredData={filteredIconData.data?.data ?? []}
      itemTemplate={iconOptionTemplate}
      onChange={handleOnChange}
      onFilter={setFilter}
      onPaging={setPaging}
      optionLabel="name"
      optionValue="source"
      selectName='Icon'
      selectedTemplate={selectedTemplate}
    />
  );
};

IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

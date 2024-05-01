import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import { getIconUrl } from '@lib/common/common';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import { IconFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useEffect, useRef, useState } from 'react';

type IconSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  readonly useDefault?: boolean;
  readonly large?: boolean;
};

const IconSelector = ({ enableEditMode = true, large = false, value, disabled, editable = true, onChange, useDefault }: IconSelectorProperties) => {
  const [origValue, setOrigValue] = useState<string | undefined>(undefined);
  const [iconSource, setIconSource] = useState<string | undefined>(undefined);
  const [iconDto, setIconDto] = useState<IconFileDto | undefined>(undefined);

  const query = useGetIcons();
  const op = useRef<OverlayPanel>(null);

  const closeOverlay = () => op.current?.hide();

  const handleOnChange = (source: IconFileDto) => {
    if (!source?.Source) {
      return;
    }

    setIconSource(source.Source);
    setOrigValue(source.Source);
    setIconDto(source);

    closeOverlay();
    onChange && onChange(source.Source);
  };

  useEffect(() => {
    if (value && origValue === undefined) {
      setOrigValue(value);
      setIconSource(value);
      const icon = query.data?.find((i) => i.Source === value);
      setIconDto(icon);
    }
  }, [origValue, query.data, value]);

  const selectedTemplate = () => {
    const size = large ? 'icon-template-lg' : 'icon-template';
    if (iconSource === null)
      return (
        <div className={`flex ${size} justify-content-center align-items-center`}>
          <img className="no-border" alt="Icon logo" src="/images/default.png" />
        </div>
      );

    const iconUrl = iconSource ? getIconUrl(iconSource, '/images/default.png', false) : '';

    if (!iconUrl) {
      return (
        <div className={`flex ${size} justify-content-center align-items-center`}>
          <img className="no-border" alt="Icon logo" src="/images/default.png" />
        </div>
      );
    }

    return (
      <div className={`flex ${size} justify-content-center align-items-center`}>
        <img className="no-border" alt="Icon logo" src={iconUrl} />
      </div>
    );
  };

  const itemTemplate = (icon: IconFileDto) => {
    if (icon === null) return <div />;

    const iconUrl = icon ? getIconUrl(icon.Source, '/images/default.png', false) : '';

    if (!iconUrl) {
      return <div className="no-text"></div>;
    }

    return (
      <div className="flex icon-template justify-content-start align-items-center">
        <img
          src={iconUrl}
          alt={icon.Name}
          onError={(e) => {
            (e.currentTarget as HTMLImageElement).src = '/images/default.png';
          }}
          loading="lazy"
        />

        <div className="text-xs pl-2">{icon.Name}</div>
      </div>
    );
  };

  if (!enableEditMode) {
    const iconUrl = getIconUrl(iconSource ?? '', '/images/default.png', false);

    return <img alt="logo" className="iconselector" src={iconUrl} loading="lazy" />;
  }

  const loading = query.isError || query.isLoading || !query.data;

  if (loading) {
    return (
      <div className="iconselector m-0 p-0">
        <ProgressSpinner />
      </div>
    );
  }

  return (
    <SMOverlay
      buttonTemplate={selectedTemplate()}
      title="ICONS"
      widthSize="3"
      icon="pi-upload"
      buttonClassName="icon-green-filled"
      buttonLabel="Icons"
      header={<></>}
    >
      <SMScroller
        autoFocus
        filter
        filterBy="Name"
        data={query.data}
        dataKey="Source"
        itemSize={26}
        onChange={(e) => handleOnChange(e)}
        itemTemplate={itemTemplate}
        scrollHeight={250}
        value={iconDto}
      />
    </SMOverlay>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

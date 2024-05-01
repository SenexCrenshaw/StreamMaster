import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import { getIconUrl } from '@lib/common/common';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import { IconFileDto } from '@lib/smAPI/smapiTypes';
import useSettings from '@lib/useSettings';
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
};

const IconSelector = ({ enableEditMode = true, value, disabled, editable = true, onChange, useDefault }: IconSelectorProperties) => {
  const [origValue, setOrigValue] = useState<string | undefined>(undefined);
  const [iconSource, setIconSource] = useState<string | undefined>(undefined);
  const [iconDto, setIconDto] = useState<IconFileDto | undefined>(undefined);
  const setting = useSettings();
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
    if (iconSource === null) return <div />;

    const iconUrl = iconSource ? getIconUrl(iconSource, setting.defaultIcon, false) : '';

    if (!iconUrl) {
      return <div className="no-text">XXX</div>;
    }

    return (
      <div className="icon-template">
        <img className="no-border" alt="Icon logo" src={iconUrl} />
      </div>
    );
  };

  const itemTemplate = (icon: IconFileDto) => {
    if (icon === null) return <div />;

    const iconUrl = icon ? getIconUrl(icon.Source, setting.defaultIcon, false) : '';

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
    const iconUrl = getIconUrl(iconSource ?? '', setting.defaultIconUrl, false);

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
      title="EPG FILES"
      widthSize="5"
      icon="pi-upload"
      buttonClassName="icon-green-filled"
      buttonLabel="EPG"
      header={<></>}
    >
      <SMScroller
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

    // <div className="iconselector">
    //   <div
    //     onClick={(e) => {
    //       op.current?.toggle(e);
    //     }}
    //   >
    //     {selectedTemplate()}
    //   </div>

    //   <OverlayPanel ref={op} className="iconselector-panel" onHide={() => {}}>
    //     <div className="w-full h-full ">
    //       <SMScroller
    //         filter
    //         filterBy="Name"
    //         data={query.data}
    //         dataKey="Source"
    //         itemSize={26}
    //         onChange={(e) => handleOnChange(e)}
    //         itemTemplate={itemTemplate}
    //         scrollHeight={250}
    //         value={iconDto}
    //       />
    //     </div>
    //   </OverlayPanel>
    // </div>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

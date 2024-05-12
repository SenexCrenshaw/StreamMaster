import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import { getIconUrl } from '@lib/common/common';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import { IconFileDto } from '@lib/smAPI/smapiTypes';
import { OverlayPanel } from 'primereact/overlaypanel';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useEffect, useMemo, useRef, useState } from 'react';

type IconSelectorProperties = {
  readonly darkBackGround?: boolean;
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly label?: string;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  readonly useDefault?: boolean;
  readonly large?: boolean;
};

const IconSelector = ({
  enableEditMode = true,
  darkBackGround = false,
  large = false,
  label,
  value,
  disabled,
  editable = true,
  onChange,
  useDefault
}: IconSelectorProperties) => {
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

    // setIconSource(source.Source);
    closeOverlay();
    onChange && onChange(source.Source);
  };

  useEffect(() => {
    if (value === undefined) return;

    if (iconSource && iconSource === origValue) {
      setIconSource(value);
      setOrigValue(value);
      const icon = query.data?.find((i) => i.Source === value);
      setIconDto(icon);
      return;
    }
    if (iconSource && iconSource !== value) {
      setIconSource(iconSource);
      setOrigValue(iconSource);
      const icon = query.data?.find((i) => i.Source === iconSource);
      setIconDto(icon);
      return;
    } else {
      setIconSource(value);
      setOrigValue(value);
      const icon = query.data?.find((i) => i.Source === value);
      // console.log(icon);
      setIconDto(icon);
      return;
    }
  }, [iconSource, origValue, query.data, value]);

  const buttonTemplate = useMemo(() => {
    const iconUrl = iconSource ? getIconUrl(iconSource, '/images/default.png', false) : '/images/default.png';

    if (large) {
      return (
        <div className="flex justify-content-center align-items-center w-full">
          <img className="icon-template-lg dark-background" alt="Icon logo" src={iconUrl} />
        </div>
      );
    }

    return (
      <div className="sm-icon-selector flex icon-button-template justify-content-center align-items-center w-full">
        <img className="no-border" alt="Icon logo" src={iconUrl} />
      </div>
    );
  }, [iconSource, large]);

  const itemTemplate = (icon: IconFileDto) => {
    if (icon === null) return <div />;

    const iconUrl = icon ? getIconUrl(icon.Source, '/images/default.png', false) : '';

    if (!iconUrl) {
      return <div className="no-text"></div>;
    }

    return (
      <div className="w-full flex flex-row align-items-center justify-content-between p-row-odd">
        <div className="flex flex-row align-items-center h-32 p-row-odd">
          <img
            className="icon-template"
            src={iconUrl}
            alt={icon.Name}
            onError={(e) => {
              (e.currentTarget as HTMLImageElement).src = '/images/default.png';
            }}
            loading="lazy"
          />
        </div>
        <div className="w-6">
          <div className="text-xs pl-3">{icon.Name}</div>
        </div>
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
    <>
      <div className="flex flex-column align-items-start icon-label">
        {label && (
          <>
            <label>{label}</label>
            <div className="pt-small" />
          </>
        )}
      </div>
      <SMOverlay
        buttonFlex
        buttonDarkBackground={darkBackGround}
        buttonTemplate={buttonTemplate}
        title="ICONS"
        widthSize="3"
        buttonLabel="Icons"
        header={<></>}
      >
        <SMScroller
          className="icon-selector"
          autoFocus
          filter
          filterBy="Name"
          data={query.data}
          dataKey="Source"
          itemSize={32}
          onChange={(e) => handleOnChange(e)}
          itemTemplate={itemTemplate}
          scrollHeight={250}
          value={iconDto}
        />
      </SMOverlay>
    </>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

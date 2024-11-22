import SMDropDown from '@components/sm/SMDropDown';
import { getIconUrl } from '@lib/common/common';
import useGetLogos from '@lib/smAPI/Logos/useGetLogos';

import { LogoFileDto, SMFileTypes } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useEffect, useMemo, useState } from 'react';

type IconSelectorProperties = {
  readonly className?: string;
  readonly darkBackGround?: boolean;
  readonly enableEditMode?: boolean;
  readonly autoPlacement?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly large?: boolean;
  readonly isCustomPlayList?: boolean;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const IconSelector = ({
  className,
  darkBackGround = false,
  enableEditMode = true,
  autoPlacement = false,
  isLoading = false,
  isCustomPlayList = false,
  label,
  large = false,
  value,
  onChange
}: IconSelectorProperties) => {
  const [origValue, setOrigValue] = useState<string | undefined>(undefined);
  const [iconSource, setIconSource] = useState<string | undefined>(undefined);
  const [iconDto, setIconDto] = useState<LogoFileDto | undefined>(undefined);

  const query = useGetLogos();

  const handleOnChange = (source: LogoFileDto) => {
    if (!source?.Source) {
      return;
    }

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
      setIconDto(icon);
      return;
    }
  }, [iconSource, origValue, query.data, value]);

  // Logger.debug('IconSelector', 'IconSelector', value);

  const buttonTemplate = useMemo(() => {
    if (iconSource === undefined) {
      return '/images/default.png';
    }

    const iconUrl = iconSource
      ? getIconUrl(iconSource, '/images/default.png', false, isCustomPlayList === true ? SMFileTypes.CustomPlayList : null)
      : '/images/default.png';

    if (large) {
      return (
        <div className="flex justify-content-center align-items-center w-full">
          <img className="icon-template-lg dark-background" alt="Icon logo" src={iconUrl} />
        </div>
      );
    }

    return (
      <div className="flex icon-button-template justify-content-center align-items-center w-full">
        <img alt="Icon logo" src={iconUrl} />
      </div>
    );
  }, [iconSource, isCustomPlayList, large]);

  const itemTemplate = (icon: LogoFileDto) => {
    if (icon === null) return <div />;

    const iconUrl = icon ? getIconUrl(icon.Source, '/images/default.png', false, icon.SMFileType) : '';

    if (!iconUrl) {
      return <div className="no-text"></div>;
    }

    return (
      <div className="w-full flex flex-row align-items-center justify-content-between p-row-odd">
        <div className="flex flex-row align-items-center p-row-odd">
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

  const getDiv = useMemo(() => {
    let div = 'w-full';
    if (large) {
      div += ' sm-iconselector-lg width-100 ';
    }

    if (label) {
      div += ' flex-column';
    }

    return div;
  }, [label, large]);

  if (!enableEditMode) {
    const iconUrl = getIconUrl(iconSource ?? '', '/images/default.png', false, isCustomPlayList === true ? SMFileTypes.CustomPlayList : null);
    return (
      <div className="w-full flex flex-row align-items-center justify-content-center p-row-odd">
        <img
          alt="Logo Icon"
          className="icon-template"
          src={iconUrl}
          onError={(e) => {
            (e.currentTarget as HTMLImageElement).src = '/images/default.png';
          }}
          loading="lazy"
        />
      </div>
    );
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
    <div className={className}>
      {label && (
        <div className="flex flex-column align-items-start">
          <label className="pl-15">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <div className={getDiv}>
        <SMDropDown
          buttonLabel="LOGOS"
          buttonLargeImage={large}
          buttonDarkBackground={darkBackGround}
          buttonTemplate={buttonTemplate}
          data={query.data}
          dataKey="Source"
          filter
          filterBy="Source"
          autoPlacement={autoPlacement}
          itemSize={32}
          itemTemplate={itemTemplate}
          buttonIsLoading={isLoading}
          onChange={(e) => {
            handleOnChange(e);
          }}
          title="LOGOS"
          value={iconDto}
          contentWidthSize="3"
        />
      </div>
    </div>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

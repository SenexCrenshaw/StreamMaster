import SMDropDown from '@components/sm/SMDropDown';
import { getIconUrl } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import useGetLogos from '@lib/smAPI/Logos/useGetLogos';
import { LogoFileDto, SMFileTypes } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

type IconSelectorProps = {
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

const IconSelector: React.FC<IconSelectorProps> = ({
  className = '',
  darkBackGround = false,
  enableEditMode = true,
  autoPlacement = false,
  isLoading = false,
  label,
  large = false,
  isCustomPlayList = false,
  value,
  onChange
}) => {
  const [iconSource, setIconSource] = useState<string | undefined>(value);
  const [iconDto, setIconDto] = useState<LogoFileDto | undefined>(undefined);

  const query = useGetLogos();

  // Update icon source and details when value changes
  useEffect(() => {
    const selectedIcon = query.data?.find((icon) => icon.Source === value);
    setIconSource(value);
    setIconDto(selectedIcon);
  }, [value, query.data]);

  const loading = useMemo(() => query.isLoading || query.isError || !query.data, [query.isLoading, query.isError, query.data]);
  Logger.debug('IconSelector', value, loading);

  const cacheBustedUrl = useCallback((iconUrl: string) => {
    const uniqueTimestamp = Date.now(); // Generate a unique timestamp
    return `${iconUrl}?_=${uniqueTimestamp}`;
  }, []); // Regenerate only when iconUrl changes

  const buttonTemplate = () => {
    const fallbackUrl = '/images/default.png';
    let iconUrl = iconSource ? getIconUrl(iconSource, fallbackUrl, false, isCustomPlayList ? SMFileTypes.CustomPlayList : null) : fallbackUrl;
    iconUrl = cacheBustedUrl(iconUrl);

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
  };

  const itemTemplate = (icon: LogoFileDto) => {
    const fallbackUrl = '/images/default.png';
    const iconUrl = getIconUrl(icon.Source, fallbackUrl, false, icon.SMFileType);
    // Logger.debug('IconSelector', icon, iconUrl);

    return (
      <div className="w-full flex flex-row align-items-center justify-content-between p-row-odd">
        <div className="sm-w-6">
          <img
            className="icon-template"
            src={iconUrl}
            alt={icon.Name}
            onError={(e) => ((e.currentTarget as HTMLImageElement).src = fallbackUrl)}
            loading="lazy"
          />
        </div>
        <div className="sm-w-6">
          <div className="text-xs pl-3">{icon.Name}</div>
        </div>
      </div>
    );
  };

  const containerClass = useMemo(() => {
    let baseClass = 'w-full';
    if (large) baseClass += ' sm-iconselector-lg width-100';
    if (label) baseClass += ' flex-column';
    return baseClass;
  }, [large, label]);

  const handleIconChange = (selectedIcon: LogoFileDto) => {
    if (selectedIcon?.Source) {
      onChange?.(selectedIcon.Value);
    }
  };

  if (!enableEditMode) {
    const iconUrl = getIconUrl(iconSource ?? '', '/images/default.png', false, isCustomPlayList ? SMFileTypes.CustomPlayList : null);
    return (
      <div className="w-full flex flex-row align-items-center justify-content-center p-row-odd">
        <img
          alt="Logo Icon"
          className="icon-template"
          src={iconUrl}
          onError={(e) => ((e.currentTarget as HTMLImageElement).src = '/images/default.png')}
          loading="lazy"
        />
      </div>
    );
  }

  if (loading) {
    return <div className="iconselector m-0 p-0">{query.isError ? <span>Error loading icons</span> : <ProgressSpinner />}</div>;
  }

  return (
    <div className={className}>
      {label && (
        <div className="flex flex-column align-items-start">
          <label className="pl-15" htmlFor="icon-selector-dropdown">
            {label.toUpperCase()}
          </label>
          <div className="pt-small" />
        </div>
      )}
      <div className={containerClass}>
        <SMDropDown
          buttonLabel="LOGOS"
          buttonLargeImage={large}
          buttonDarkBackground={darkBackGround}
          buttonTemplate={buttonTemplate()}
          data={query.data}
          dataKey="Source"
          filter
          filterBy="Source"
          autoPlacement={autoPlacement}
          itemSize={32}
          itemTemplate={itemTemplate}
          buttonIsLoading={isLoading}
          onChange={handleIconChange}
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

import SMDropDown from '@components/sm/SMDropDown';

import { baseHostURL, isDev } from '@lib/settings';
import useGetLogos from '@lib/smAPI/Logos/useGetLogos';
import { CustomLogoDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { CustomLogosAddDialog } from './CustomLogosAddDialog';
import { getIconUrl, SMLogo } from './iconUtil';

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
  const [iconDto, setIconDto] = useState<CustomLogoDto | undefined>(undefined);

  const query = useGetLogos();

  // Update icon source and details when value changes
  useEffect(() => {
    const selectedIcon = query.data?.find((icon) => icon.Source === value);
    setIconSource(value);
    setIconDto(selectedIcon);
    if (value === '') {
      console.log('IconSelector: value is empty');
    }
    console.log(value);
  }, [value, query.data]);

  // const loading = useMemo(() => query.isLoading || query.isError || !query.data, [query.isLoading, query.isError, query.data]);

  const cacheBustedUrl = useCallback((iconUrl: string) => {
    const uniqueTimestamp = Date.now(); // Generate a unique timestamp
    return `${iconUrl}?_=${uniqueTimestamp}`;
  }, []); // Regenerate only when iconUrl changes

  const buttonTemplate = () => {
    let iconUrl = cacheBustedUrl(getIconUrl(iconSource));

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

  const itemTemplate = (icon: CustomLogoDto) => {
    let iconUrl = icon.Source;
    if (!iconUrl.startsWith('http')) {
      iconUrl = `${isDev ? `${baseHostURL}` : '/'}${iconUrl}`;
    }

    return (
      <div className="w-full flex flex-row align-items-center justify-content-between p-row-odd">
        <div className="flex sm-w-4rem icon-selector-template">
          <img
            // className="icon-template"
            src={iconUrl}
            alt={icon.Name}
            onError={(e) => ((e.currentTarget as HTMLImageElement).src = SMLogo)}
            loading="lazy"
          />
        </div>
        <div className="w-9">
          <div className={`${icon.Name.length > 30 ? 'sm-text-xs' : 'sm-text-lg'} sm-w-1rem`}>{icon.Name}</div>
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

  const handleIconChange = (selectedIcon: CustomLogoDto) => {
    if (selectedIcon?.Source) {
      onChange?.(selectedIcon.Value);
    }
  };

  const headerRight = useMemo((): React.ReactNode => {
    return (
      <div className="flex w-12 gap-1 justify-content-end align-content-center">
        <CustomLogosAddDialog />
      </div>
    );
  }, []);

  if (!enableEditMode) {
    const iconUrl = getIconUrl(iconSource);
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
          autoPlacement={autoPlacement}
          buttonDarkBackground={darkBackGround}
          buttonIsLoading={isLoading}
          buttonLabel="LOGOS"
          buttonLargeImage={large}
          buttonTemplate={buttonTemplate()}
          contentWidthSize="3"
          data={query.data}
          dataKey="Source"
          filter
          filterBy="Name"
          header={headerRight}
          itemSize={32}
          itemTemplate={itemTemplate}
          onChange={handleIconChange}
          title="LOGOS"
          value={iconDto}
        />
      </div>
    </div>
  );
};

IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

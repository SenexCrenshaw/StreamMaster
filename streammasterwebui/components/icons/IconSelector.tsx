import { getIconUrl } from '@lib/common/common';
import useGetIcons from '@lib/smAPI/Icons/useGetIcons';
import useSettings from '@lib/useSettings';
import { OverlayPanel } from 'primereact/overlaypanel';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useEffect, useRef, useState } from 'react';
import IconGrid from './IconGrid';

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
  const setting = useSettings();
  const query = useGetIcons();
  const op = useRef<OverlayPanel>(null);

  useEffect(() => {
    if (value && origValue === undefined) {
      setOrigValue(value);
      setIconSource(value);
    }
  }, [origValue, value]);

  const selectedTemplate = () => {
    if (iconSource === null) return <div />;

    const iconUrl = iconSource ? getIconUrl(iconSource, setting.defaultIcon, false) : '';

    if (!iconUrl) {
      return <div className="no-text">XXX</div>;
    }

    return (
      <div className="flex icon-template align-content-center justify-content-center align-items-center">
        <img alt="Icon logo" src={iconUrl} />
      </div>
    );
  };

  const closeOverlay = () => op.current?.hide();

  const handleOnChange = (source: string) => {
    if (!source) {
      return;
    }

    setIconSource(source);
    setOrigValue(source);

    closeOverlay();
    onChange && onChange(source);
  };

  if (!enableEditMode) {
    const iconUrl = getIconUrl(iconSource ?? '', setting.defaultIconUrl, false);

    return <img alt="logo" className="default-icon" src={iconUrl} loading="lazy" />;
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
    <div className="iconselector">
      <div
        onClick={(e) => {
          op.current?.toggle(e);
        }}
      >
        {selectedTemplate()}
      </div>

      <OverlayPanel ref={op} className="iconselector-panel" onHide={() => {}}>
        <IconGrid
          onClick={(e) => {
            console.log(e);
            handleOnChange(e);
          }}
          iconSource={iconSource}
        />
      </OverlayPanel>
    </div>
  );
};
IconSelector.displayName = 'IconSelector';

export default React.memo(IconSelector);

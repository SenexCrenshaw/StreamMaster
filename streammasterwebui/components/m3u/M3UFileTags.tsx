import { Button } from 'primereact/button';
import { Chips } from 'primereact/chips';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { useMemo, useRef, useState } from 'react';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  vodTags?: string[];
  onChange: (vodTags: string[]) => void;
}

const M3UFileTags = ({ m3uFileDto, onChange, vodTags }: M3UFileTagsProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorReference = useRef(null);

  const [isOpen, setIsOpen] = useState(false);

  const intTags = useMemo((): string[] => {
    const tags = m3uFileDto ? m3uFileDto.vodTags : vodTags ?? [];

    return tags;
  }, [m3uFileDto, vodTags]);

  const buttonTags = useMemo((): string => {
    if (intTags.length > 0 && intTags.length < 3) return intTags.join(', ');

    return intTags.length + ' Tags';
  }, [intTags]);

  return (
    <div className="w-full bordered-text tag-editor p-0 m-0" ref={anchorReference}>
      <Button
        className="text-sm tag-editor"
        icon="pi pi-chevron-down"
        label={buttonTags}
        text={true}
        onClick={(e) => {
          if (isOpen) {
            op.current?.hide();
          } else {
            op.current?.show(null, anchorReference.current);
          }
          setIsOpen(!isOpen);
        }}
      />
      <OverlayPanel ref={op} onHide={() => setIsOpen(false)} style={{ width: '20rem' }}>
        <Chips
          autoFocus
          id="chips"
          value={intTags}
          onChange={(e) => {
            onChange(e.value ?? []);
          }}
        />
      </OverlayPanel>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTags);

import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { Button } from 'primereact/button';
import { Chips } from 'primereact/chips';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { useMemo, useRef, useState } from 'react';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  onChange: (vodTags: string[]) => void;
  vodTags?: string[];
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
    <div className="pb-3">
      <div id="name" className="text-xs text-500">
        URL REGEX
      </div>
      <div className="w-full tag-editor p-0 m-0 default-border" ref={anchorReference}>
        <OverlayPanel ref={op} onHide={() => setIsOpen(false)}>
          <Chips
            autoFocus
            id="chips"
            value={intTags}
            onChange={(e) => {
              onChange(e.value ?? []);
            }}
          />
        </OverlayPanel>

        <div>
          <Button
            className="text-sm tag-editor"
            icon="pi pi-chevron-down"
            iconPos="right"
            id="tag-Button"
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
        </div>
      </div>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTags);

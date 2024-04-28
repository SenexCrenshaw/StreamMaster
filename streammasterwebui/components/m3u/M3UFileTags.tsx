import DownArrowButton from '@components/buttons/DownArrowButton';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { Chips } from 'primereact/chips';
import { OverlayPanel } from 'primereact/overlaypanel';
import React, { useMemo, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  onChange: (vodTags: string[]) => void;
  vodTags?: string[];
}

const M3UFileTags = ({ m3uFileDto, onChange, vodTags }: M3UFileTagsProperties) => {
  const op = useRef<OverlayPanel>(null);
  const anchorReference = useRef(null);
  const uuid = uuidv4();
  const [isOpen, setIsOpen] = useState(false);

  const intTags = useMemo((): string[] => {
    const tags = m3uFileDto ? m3uFileDto.VODTags : vodTags ?? [];

    return tags;
  }, [m3uFileDto, vodTags]);

  const buttonTags = useMemo((): string => {
    if (intTags.length > 0 && intTags.length < 3) return intTags.join(', ');

    return intTags.length + ' Tags';
  }, [intTags]);

  return (
    <div className="w-12 sm-M3UFileTags">
      <label className="flex text-xs text-default-color w-full justify-content-center align-items-center" htmlFor={uuid}>
        URL REGEX
      </label>
      <div className="layout-padding-bottom"></div>
      <div id={uuid} className="w-full tag-editor p-0 m-0 input-border" ref={anchorReference}>
        <OverlayPanel ref={op} onHide={() => setIsOpen(false)} className="w-2">
          <div className="flex w-full align-items-center">
            <Chips
              className="w-full"
              autoFocus
              id="chips"
              value={intTags}
              onChange={(e) => {
                console.log('change', e.value);
                onChange(e.value ?? []);
              }}
            />
          </div>
        </OverlayPanel>

        <div className="flex w-full">
          <DownArrowButton
            className="w-full"
            label={buttonTags}
            tooltip=""
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

M3UFileTags.displayName = 'M3UFileTags';

export default React.memo(M3UFileTags);

import SMPopUp from '@components/sm/SMPopUp';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { Chips } from 'primereact/chips';
import React, { useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

export interface M3UFileTagsProperties {
  m3uFileDto?: M3UFileDto;
  onChange: (vodTags: string[]) => void;
  vodTags?: string[];
}

const M3UFileTags = ({ m3uFileDto, onChange, vodTags }: M3UFileTagsProperties) => {
  const uuid = uuidv4();

  const intTags = useMemo((): string[] => {
    const tags = m3uFileDto ? m3uFileDto.VODTags : vodTags ?? [];

    return tags;
  }, [m3uFileDto, vodTags]);

  const buttonTags = useMemo((): string => {
    if (intTags.length > 0 && intTags.length < 3) return intTags.join(', ');

    return intTags.length + ' Tags';
  }, [intTags]);

  const buttonTemplate = useMemo(() => {
    return (
      <div className="flex w-full justify-content-center align-items-center">
        <div className="text-container pl-1" style={{ color: '#d3d88e' }}>
          {buttonTags}
        </div>
      </div>
    );
  }, [buttonTags]);

  return (
    <div className="sm-w-12">
      <label className="flex text-xs text-default-color w-full justify-content-start align-items-center pl-2 w-full" htmlFor={uuid}>
        URL REGEX
      </label>
      <div id={uuid} className="stringeditor">
        <SMPopUp
          className="w-full"
          buttonDarkBackground
          buttonTemplate={buttonTemplate}
          title="URL Regex Tags"
          contentWidthSize="3"
          icon="pi-chevron-down"
          zIndex={13}
        >
          <div className="p-fluid h-3rem w-full">
            <Chips
              autoFocus
              value={intTags}
              onChange={(e) => {
                onChange(e.value ?? []);
              }}
              variant="filled"
            />
          </div>
        </SMPopUp>
      </div>
    </div>
  );
};

M3UFileTags.displayName = 'M3UFileTags';

export default React.memo(M3UFileTags);

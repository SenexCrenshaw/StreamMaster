import SMDropDown from '@components/sm/SMDropDown';
import { getEnumValueByKey, getHandlersOptions } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';
import { M3UFileDto, M3UKey } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useMemo } from 'react';
import { isNumber } from '@lib/common/common';

import { v4 as uuidv4 } from 'uuid';

export interface M3UKeyDialogProperties {
  m3uFileDto: M3UFileDto;
  onChange: (M3UKey: M3UKey) => void;
}

const M3UKeyDialog = ({ m3uFileDto, onChange }: M3UKeyDialogProperties) => {
  const uuid = uuidv4();

  const buttonTemplate = useMemo((): ReactNode => {
    const m3uField = m3uFileDto?.M3UKey;

    const name = isNumber(m3uField) ? getEnumValueByKey(M3UKey, m3uField.toString() as keyof typeof M3UKey) : m3uField ?? ''; // Use nullish coalescing for cleaner fallback

    return (
      <div className="sm-epg-selector">
        <div className="text-container" style={{ paddingLeft: '0.12rem' }}>
          {name}
        </div>
      </div>
    );
  }, [m3uFileDto?.M3UKey]);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  const onChanged = (e: SelectItem) => {
    Logger.debug('M3UFileTags', e);
    const ret = getEnumValueByKey(M3UKey, (e.label as keyof typeof M3UKey) ?? null);
    onChange(ret);
  };

  if (m3uFileDto === null || m3uFileDto === undefined) {
    return <></>;
  }

  return (
    <div className="sm-w-12">
      <label className="flex text-xs text-default-color w-full justify-content-start align-items-center pl-2 w-full" htmlFor={uuid}>
        Key to identify each M3U stream
      </label>
      <div id={uuid} className="stringeditor">
        <div className="sm-w-12">
          <SMDropDown
            buttonDarkBackground
            buttonDisabled={false}
            buttonTemplate={buttonTemplate}
            contentWidthSize="2"
            data={getHandlersOptions(M3UKey)}
            dataKey="label"
            info=""
            itemTemplate={itemTemplate}
            onChange={(e) => onChanged(e)}
            propertyToMatch="label"
            scrollHeight="20vh"
            title={'M3u key Mapping'}
            value={{ id: getEnumValueByKey(M3UKey, (m3uFileDto?.M3UKey.toString() as keyof typeof M3UKey) ?? null), label: m3uFileDto?.M3UKey }}
            zIndex={11}
          />
        </div>
      </div>
    </div>
  );
};

M3UKeyDialog.displayName = 'M3UKeyDialog';

export default React.memo(M3UKeyDialog);

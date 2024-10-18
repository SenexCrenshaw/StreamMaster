import SMDropDown from '@components/sm/SMDropDown';
import { isNumber } from '@lib/common/common';
import { getEnumValueByKey, getHandlersOptions } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';
import { M3UFileDto, M3UField } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

export interface M3UFieldNameDialogProperties {
  m3uFileDto: M3UFileDto;
  onChange: (m3uName: M3UField) => void;
}

const M3UFieldNameDialog = ({ m3uFileDto, onChange }: M3UFieldNameDialogProperties) => {
  const uuid = uuidv4();

  Logger.debug('M3UFieldNameDialog', m3uFileDto);

  const buttonTemplate = useMemo((): ReactNode => {
    const m3uField = m3uFileDto?.M3UName;

    const name = isNumber(m3uField) ? getEnumValueByKey(M3UField, m3uField.toString() as keyof typeof M3UField) : m3uField ?? ''; // Use nullish coalescing for cleaner fallback

    return (
      <div className="sm-epg-selector">
        <div className="text-container" style={{ paddingLeft: '0.12rem' }}>
          {name}
        </div>
      </div>
    );
  }, [m3uFileDto?.M3UName]);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  const onChanged = (e: SelectItem) => {
    Logger.debug('M3UFileTags', e);
    const ret = getEnumValueByKey(M3UField, (e.label as keyof typeof M3UField) ?? null);
    onChange(ret);
  };

  if (m3uFileDto === null || m3uFileDto === undefined) {
    return <></>;
  }

  return (
    <div className="sm-w-12">
      <label className="flex text-xs text-default-color w-full justify-content-start align-items-center pl-2 w-full" htmlFor={uuid}>
        Channel Name Field
      </label>
      <div id={uuid} className="stringeditor">
        <div className="sm-w-12">
          <SMDropDown
            buttonDarkBackground
            buttonDisabled={false}
            buttonTemplate={buttonTemplate}
            contentWidthSize="2"
            data={getHandlersOptions(M3UField)}
            dataKey="label"
            info=""
            itemTemplate={itemTemplate}
            onChange={(e) => onChanged(e)}
            propertyToMatch="label"
            scrollHeight="20vh"
            title={'Channel Name Field'}
            value={{ id: getEnumValueByKey(M3UField, (m3uFileDto?.M3UName.toString() as keyof typeof M3UField) ?? null), label: m3uFileDto?.M3UName }}
            zIndex={11}
          />
        </div>
      </div>
    </div>
  );
};

M3UFieldNameDialog.displayName = 'M3UFieldNameDialog';

export default React.memo(M3UFieldNameDialog);

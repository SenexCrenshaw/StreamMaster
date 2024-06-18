import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { FileOutputProfileDto, M3UOutputProfile } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';
import FileProfileValueDropDown from './FileProfileValueDropDown';

export interface FileProfileColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number;
}

interface IntFileProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
  readonly width?: number;
}

export const useFileProfileColumnConfig = ({ field, header, width = 80 }: IntFileProfileColumnConfigProps) => {
  const bodyTemplate = useCallback(
    (fileOutputProfile: FileOutputProfileDto) => {
      var key = field as keyof M3UOutputProfile;
      let value = fileOutputProfile.M3UOutputProfile[key] as string;

      if (fileOutputProfile.IsReadOnly === true) {
        return (
          <div className="sm-epg-selector">
            <div className="text-container pl-1">{value}</div>
          </div>
        );
      }

      Logger.debug('value', key, value, field);
      return <FileProfileValueDropDown header={header} value={value} field={field} name={fileOutputProfile.Name} />;
    },
    [field, header]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: width
  };

  return columnConfig;
};

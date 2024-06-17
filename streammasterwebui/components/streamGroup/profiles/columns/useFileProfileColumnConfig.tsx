import SMDropDown from '@components/sm/SMDropDown';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { FileOutputProfile, M3UOutputProfile, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import { ReactNode, useCallback, useMemo } from 'react';

// interface IdName {
//   Id: number;
//   Name: string;
// }

interface useFileProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
}

export const useFileProfileColumnConfig = ({ field, header }: useFileProfileColumnConfigProps) => {
  // const itemTemplate = useCallback((option: SelectItem) => {
  //   if (option === undefined) {
  //     return null;
  //   }
  //   Logger.debug('itemTemplate', option);
  //   return (
  //     <>
  //       <div className="text-container">{option?.label ?? ''}</div>
  //     </>
  //   );
  // }, []);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    Logger.debug('itemTemplate', option);
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(ValidM3USetting)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: ValidM3USetting[key as keyof typeof ValidM3USetting]
      }));

    return options;
  }, []);

  // const validM3USettings = useMemo(() => {
  //   return [
  //     { Id: 1, Name: 'None' },
  //     { Id: 2, Name: 'Name' },
  //     { Id: 3, Name: 'Group' },
  //     { Id: 4, Name: 'EPGID' }
  //   ] as IdName[];
  // }, []);

  const buttonTemplate = useCallback((name: string): ReactNode => {
    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{name}</div>
      </div>
    );
  }, []);

  const bodyTemplate = useCallback(
    (fileOutputProfile: FileOutputProfile) => {
      var key = field as keyof M3UOutputProfile;
      let value = fileOutputProfile.M3UOutputProfile[key] as string;

      if (fileOutputProfile.IsReadOnly === true) {
        return (
          <div className="sm-epg-selector">
            <div className="text-container pl-1">{value}</div>
          </div>
        );
      }

      Logger.debug('value', key, value);
      return (
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate(value)}
          contentWidthSize="2"
          data={getHandlersOptions}
          dataKey="label"
          info=""
          itemTemplate={itemTemplate}
          onChange={async (e: any) => {}}
          optionValue="label"
          title={header + ' Mapping'}
          value={value}
        />
      );
    },
    [buttonTemplate, field, getHandlersOptions, header, itemTemplate]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: 80
  };

  return columnConfig;
};

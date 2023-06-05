
import { type DataTableValue } from 'primereact/datatable';
import { type CSSProperties } from 'react';
import React from 'react';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

const DataSelectorPicker = <T extends DataTableValue,>(props: DataSelectorPickerProps<T>) => {

  const [selection, setSelection] = React.useState<T[]>([] as T[]);

  const onSelectionChange = React.useCallback((value: T[]) => {
    setSelection(value);
    if (props?.onSelectionChange === undefined) return;

    props.onSelectionChange(value);
  }, [props]);


  // const onTargetSelectionsChange = React.useCallback((value: T[]) => {
  //   if (Array.isArray(value)) {
  //     props?.onTargetSelectionChange?.(value);
  //   }

  // }, [props]);

  React.useMemo(() => {
    if (props?.selection === undefined) {
      return;
    }

    if (props.selection instanceof Array) {
      const sel = props.selection.filter((obj) => obj !== null && obj !== undefined) as T[];
      setSelection(sel);
      return;
    }

    setSelection([props.selection as T]);


  }, [props.selection]);



  const dataSource = React.useMemo((): T[] | undefined => {

    if (!props.sourceDataSource) {
      return;
    }

    const idsToMoveToTop = selection.map((obj) => {
      if (!obj || obj.id === null || !obj.id || obj.id === undefined) {
        console.log('ee')
      }

      return obj.id;
    });

    const test = props.sourceDataSource.filter((obj) => !idsToMoveToTop.includes(obj.id));

    return test;

  }, [props.sourceDataSource, selection]);

  return (
    <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
      <div className='col-6'>
        <DataSelector
          columns={props.sourceColumns}
          dataSource={dataSource}
          enableState={props.sourceEnableState}
          headerLeftTemplate={props.sourceHeaderPrefixTemplate}
          headerRightTemplate={props.sourceHeaderTemplate}
          id={`${props.id}-ds-picker-source`}
          isLoading={props.isLoading}
          name={props.sourceName}
          onSelectionChange={((e) => onSelectionChange(e as T[]))}
          rightColSize={props.sourceRightColSize}
          selection={selection}
          selectionMode='multipleNoCheckBox'
          sortField={props.sourceSortField}
          style={props.sourceStyle}
        />
      </div>
      <div className='col-6 p-0'>
        <DataSelector
          columns={props.targetColumns}
          dataSource={props.targetDataSource ? props.targetDataSource : props.sourceDataSource}
          enableState={props.targetEnableState}
          headerLeftTemplate={props.targetHeaderPrefixTemplate}
          headerRightTemplate={props.targetHeaderTemplate}
          id={`${props.id}-ds-picker-target`}
          isLoading={props.isLoading}
          name={props.targetName}
          onSelectionChange={(e) => props?.onTargetSelectionChange?.(e as T[])}
          onValueChanged={(e) => props.onTargetOnValueChanged?.(e as T[])}
          reorderable={props.targetReorderable}
          rightColSize={props.targetRightColSize}
          sortField={props.targetSortField}
          style={props.targetStyle ? props.targetStyle : props.sourceStyle}
        />
      </div>
    </div>
  );
};

export type DataSelectorPickerProps<T extends DataTableValue> = {
  id: string,
  isLoading: boolean | undefined;

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onSelectionChange?: (value: T | T[] | any) => void;
  onTargetOnValueChanged?: (value: T[]) => void;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onTargetSelectionChange?: (value: T | T[] | any) => void;
  selection?: T | T[] | undefined;
  sourceColumns: ColumnMeta[];
  sourceDataSource: T[] | undefined;
  sourceEnableState?: boolean | undefined;
  sourceHeaderPrefixTemplate?: React.ReactNode | undefined;
  sourceHeaderTemplate?: React.ReactNode | undefined;
  sourceName?: string | undefined,
  sourceRightColSize?: number;
  sourceSortField?: string;
  sourceStyle?: CSSProperties | undefined;
  targetColumns: ColumnMeta[];
  targetDataSource?: T[] | undefined;
  targetEnableState?: boolean | undefined;
  targetHeaderPrefixTemplate?: React.ReactNode | undefined;
  targetHeaderTemplate?: React.ReactNode | undefined;
  targetName?: string | undefined,
  targetReorderable?: boolean | undefined;
  targetRightColSize?: number;
  targetSortField?: string;
  targetStyle?: CSSProperties | undefined;

};

export default React.memo(DataSelectorPicker);

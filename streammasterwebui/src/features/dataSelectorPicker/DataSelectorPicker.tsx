/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable react/no-unused-prop-types */

import { type DataTableValue } from 'primereact/datatable';
import { type CSSProperties } from 'react';
import React from 'react';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import { Button } from 'primereact/button';
import { getTopToolOptions } from '../../common/common';

const DataSelectorPicker = <T extends DataTableValue,>(props: DataSelectorPickerProps<T>) => {

  const [selection, setSelection] = React.useState<T[]>([] as T[]);

  const [previousSelection, setPreviousSelection] = React.useState<T[]>([] as T[]);
  const [targetDataSource, setTargetDataSource] = React.useState<T[] | undefined>(undefined);

  React.useEffect(() => {
    setTargetDataSource(props.targetDataSource);

    return;

  }, [props.sourceDataSource, props.targetDataSource]);

  const onSelectionChange = React.useCallback((value: T[], isUndo?: boolean) => {
    setPreviousSelection(selection);

    if (value instanceof Array) {
      let newSelection = [...value];
      if (isUndo !== true) {
        newSelection = selection.concat(newSelection.filter((obj) => !selection.includes(obj)));
      }

      setSelection(newSelection);
      if (props?.onSelectionChange === undefined) return;
      props.onSelectionChange(newSelection);
      return;
    }

    setSelection([value]);
    if (props?.onSelectionChange === undefined) return;

    props.onSelectionChange([value]);
  }, [props, selection]);


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

    if (!props.sourceDataSource || selection.length === 0) {
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

  const targetRightHeaderTemplate = React.useMemo(() => {
    if (props.targetHeaderTemplate && props.showUndo !== true) {
      return props.targetHeaderTemplate;
    }

    return (
      <>
        {props.targetHeaderTemplate}
        <Button
          className="ml-1"
          icon="pi pi-undo"
          onClick={() => {
            onSelectionChange(previousSelection, true);
          }
          }
          rounded
          severity="warning"
          size="small"
          style={{
            ...{
              maxHeight: "2rem",
              maxWidth: "2rem"
            }
          }}
          tooltip="Undo Last Change"
          tooltipOptions={getTopToolOptions}
        />
      </>
    );

  }, [onSelectionChange, previousSelection, props.showUndo, props.targetHeaderTemplate]);

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
          // onValueChanged={(e) => onSourceOnValueChanged(e as T[])}
          rightColSize={props.sourceRightColSize}
          selection={selection}
          selectionMode='multipleNoRowCheckBox'
          sortField={props.sourceSortField}
          style={props.sourceStyle}
        />
      </div>
      <div className='col-6 p-0'>
        <DataSelector
          columns={props.targetColumns}
          dataSource={targetDataSource}
          enableState={props.targetEnableState}
          headerLeftTemplate={props.targetHeaderPrefixTemplate}
          headerRightTemplate={targetRightHeaderTemplate}
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

DataSelectorPicker.defaultProps = {
  sourceEnableState: true,
  targetEnableState: true,

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
  showUndo?: boolean | undefined;
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

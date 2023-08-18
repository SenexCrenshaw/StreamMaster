/* eslint-disable @typescript-eslint/no-unused-vars */

import { type DataTableValue } from 'primereact/datatable';
import { type CSSProperties } from 'react';
import React from 'react';

import { Button } from 'primereact/button';
import { getTopToolOptions } from '../../common/common';
import { type PagedTableDto } from '../dataSelector2/DataSelector2';
import DataSelector2 from '../dataSelector2/DataSelector2';
import { type ColumnMeta } from '../dataSelector2/DataSelectorTypes2';

const DataSelectorPicker2 = <T extends DataTableValue,>(props: DataSelectorPicker2Props<T>) => {

  const [selection, setSelection] = React.useState<T[] | undefined>([] as T[]);

  const [previousSelection, setPreviousSelection] = React.useState<T[]>([] as T[]);


  const onSelectionChange = React.useCallback((value: T[], isUndo?: boolean) => {

    if (selection) {
      setPreviousSelection(selection);
    }

    if (value instanceof Array) {
      let newSelection = [...value];
      if (isUndo !== true) {
        if (selection) {
          newSelection = selection.concat(newSelection.filter((obj) => !selection.includes(obj)));
        }
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


  React.useEffect(() => {
    if (props?.selection === undefined) {
      return;
    }

    if (props.selection instanceof Array) {
      const sel = props.selection.filter((obj) => obj !== null && obj !== undefined) as T[];
      setSelection(sel);
      return;
    }

    setSelection([props.selection as T]);

    return () => {
      setSelection(undefined);
    }
  }, [props.selection]);


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
        <DataSelector2
          columns={props.sourceColumns}
          dataSource={props.sourceDataSource}
          headerLeftTemplate={props.sourceHeaderPrefixTemplate}
          headerRightTemplate={props.sourceHeaderTemplate}
          id={`${props.id}-ds-picker-source`}
          isLoading={props.isLoading}
          name={props.sourceName}
          onSelectionChange={((e) => onSelectionChange(e as T[]))}
          rightColSize={props.sourceRightColSize}
          selectionMode='multipleNoRowCheckBox'
          style={props.sourceStyle}
        />
      </div>
      <div className='col-6 p-0'>
        <DataSelector2
          columns={props.targetColumns}
          dataSource={props.targetDataSource}
          headerLeftTemplate={props.targetHeaderPrefixTemplate}
          headerRightTemplate={targetRightHeaderTemplate}
          id={`${props.id}-ds-picker-target`}
          isLoading={props.isLoading}
          name={props.targetName}
          onSelectionChange={(e) => {
            props?.onTargetSelectionChange?.(e as T[])
          }
          }
          onValueChanged={(e) => props.onTargetOnValueChanged?.(e as T[])}
          reorderable={props.targetReorderable}
          rightColSize={props.targetRightColSize}
          style={props.targetStyle ? props.targetStyle : props.sourceStyle}
        />
      </div>
    </div>
  );
};

DataSelectorPicker2.defaultProps = {

};


export type DataSelectorPicker2Props<T extends DataTableValue> = {
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
  sourceDataSource: PagedTableDto<T> | undefined;
  sourceHeaderPrefixTemplate?: React.ReactNode | undefined;
  sourceHeaderTemplate?: React.ReactNode | undefined;
  sourceName?: string | undefined,
  sourceRightColSize?: number;
  sourceStyle?: CSSProperties | undefined;
  targetColumns: ColumnMeta[];
  targetDataSource?: PagedTableDto<T> | undefined;
  targetHeaderPrefixTemplate?: React.ReactNode | undefined;
  targetHeaderTemplate?: React.ReactNode | undefined;
  targetName?: string | undefined,
  targetReorderable?: boolean | undefined;
  targetRightColSize?: number;
  targetStyle?: CSSProperties | undefined;

};

export default React.memo(DataSelectorPicker2);

import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetEPGFilePreviewByIdSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetEPGFilePreviewById } from './GetEPGFilePreviewByIdFetch';
import {FieldData, EPGFilePreviewDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<EPGFilePreviewDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetEPGFilePreviewById = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetEPGFilePreviewById.data);
  const error = useAppSelector((state) => state.GetEPGFilePreviewById.error ?? '');
  const isError = useAppSelector((state) => state.GetEPGFilePreviewById.isError?? false);
  const isForced = useAppSelector((state) => state.GetEPGFilePreviewById.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetEPGFilePreviewById.isLoading ?? false);

const SetIsForced = useCallback(
  (forceRefresh: boolean, query?: string): void => {
    dispatch(setIsForced({ force: forceRefresh }));
  },
  [dispatch]
);


const SetIsLoading = useCallback(
  (isLoading: boolean): void => {
    dispatch(setIsLoading({ isLoading: isLoading }));
  },
  [dispatch]
);
  useEffect(() => {
  const state = store.getState().GetEPGFilePreviewById;
  if (data === undefined && state.isLoading !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch]);


const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  data,
  error,
  isError,
  isLoading,
  Clear,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetEPGFilePreviewById;

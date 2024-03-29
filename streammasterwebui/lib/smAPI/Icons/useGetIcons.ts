import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetIcons, intSetGetIconsIsLoading, updateGetIcons } from './GetIconsSlice';
import { useEffect } from 'react';
import { fetchGetIcons } from './IconsFetch';
import { FieldData, IconFileDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<IconFileDto[] | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetIconsField: (fieldData: FieldData) => void;
  refreshGetIcons: () => void;
  setGetIconsIsLoading: (isLoading: boolean) => void;
}
const useGetIcons = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetIcons.data);
  const isLoading = useAppSelector((state) => state.GetIcons.isLoading ?? false);
  const isError = useAppSelector((state) => state.GetIcons.isError ?? false);
  const error = useAppSelector((state) => state.GetIcons.error ?? '');

  useEffect(() => {
    const test = store.getState().GetIcons;
    if (test.data !== undefined || test.isLoading) return;
    dispatch(fetchGetIcons());
  }, [data, dispatch, isLoading]);

  const setGetIconsField = (fieldData: FieldData): void => {
    dispatch(updateGetIcons({ fieldData: fieldData }));
  };

  const refreshGetIcons = (): void => {
    dispatch(clearGetIcons());
  };

  const setGetIconsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetIconsIsLoading({ isLoading: isLoading }));
  };

  return { data, error, isError, isLoading, refreshGetIcons, setGetIconsField, setGetIconsIsLoading };
};

export default useGetIcons;

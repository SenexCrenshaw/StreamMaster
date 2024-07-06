import { GetPagedM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetPagedM3UFiles = createAsyncThunk('cache/getGetPagedM3UFiles', async (query: string, thunkAPI) => {
  try {
    if (isSkipToken(query))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    if (query === undefined) return;
    const params = JSON.parse(query);
    const response = await GetPagedM3UFiles(params);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});



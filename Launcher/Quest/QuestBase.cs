using Launcher.Model;

namespace Launcher.Quest;

public abstract class QuestBase(Device d)
{
    private int _countTimeChayNv;
    protected virtual string? QuestName => null;

    protected void Delay(int milliseconds = 200)
    {
        d.Delay(milliseconds);
    }

    protected abstract Task<bool> DaNhanNv();
    protected abstract Task<bool> DiNhanNv();
    protected abstract Task<bool> DaHoanThanhNv();
    protected abstract Task<bool> DiLamNv();
    protected abstract Task<bool> DiTraNv();

    public virtual async Task<bool> ChayNv()
    {
        d.Status = "Chạy nhiệm vụ " + (QuestName ?? GetType().Name);
        _countTimeChayNv++;
        if (_countTimeChayNv >= 3) throw new Exception("Không thể chạy được nhiệm vụ " + (QuestName ?? GetType().Name));

        if (!await DaNhanNv())
        {
            d.Status = "Đi nhận nhiệm vụ " + (QuestName ?? GetType().Name);
            if (!await DiNhanNv())
                return false;
        }

        var countFail = 0;
        while (!await DaHoanThanhNv())
        {
            d.Status = "Đi làm nhiệm vụ " + (QuestName ?? GetType().Name);
            if (await DiLamNv()) continue;
            countFail++;
            if (countFail == 10) return await ChayNv();
            Delay(5000);
        }

        _countTimeChayNv = 0;
        return await DiTraNv();
    }
}
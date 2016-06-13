using System;
using Gwen.Control;
using Gwen.RichText;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-Interactive", Order = 102)]
	public class RichLabel : GUnit
	{
		private Font f1, f2, f3, f4, f5;

		public RichLabel(ControlBase parent) : base(parent)
		{
			f1 = new Font(Skin.Renderer, "Arial", 15);

			f2 = new Font(Skin.Renderer, "Times New Roman", 20);
			f2.Bold = true;

			f3 = new Font(Skin.Renderer, "Courier New", 15);
			f3.Italic = true;

			f4 = new Font(Skin.Renderer, "Times New Roman", 20);
			f4.Bold = true;
			f4.Underline = true;

			f5 = new Font(Skin.Renderer, "Courier New", 10);

			Control.RichLabel label = CreateLabel(this);

			Control.Button button = new Control.Button(this);
			button.Dock = Dock.Bottom;
			button.Width = 150;
			button.Text = "Open a Window";
			button.Clicked += OpenWindow;
		}

		public override void Dispose()
		{
			f1.Dispose();
			f2.Dispose();
			f3.Dispose();
			f4.Dispose();
			f5.Dispose();
			base.Dispose();
		}

		private Control.RichLabel CreateLabel(ControlBase parent)
		{
			Control.RichLabel label = new Control.RichLabel(parent);
			label.Dock = Dock.Fill;
			label.LinkClicked += OnLinkClicked;

			Document document = new Document();

			document.Paragraph().
				Font(f1).
					Text("This test uses Arial 15, Red. Padding. ", Color.Red).
				Font(f2).
					Text("This text uses Times New Roman Bold 20, Green. Padding. ", Color.Green).
				Font(f3).
					Text("This text uses Courier New Italic 15, Blue. Padding. ", Color.Blue).
				Font().
					Text("Test link (").
					Link("Test Link 1", "Test Link 1", Color.Blue, new Color(0xFFADD8E6)).
					Text("). ").
				Font(f2).
					Text("Test link custom font (", new Color(0xFFFF00FF)).
					Link("Test Link 2", "Test Link 2", Color.Blue, new Color(0xFFADD8E6), f4).
					Text(").", new Color(0xFFFF00FF));

			document.Paragraph().
				Font(f5).
					Text("\n").
					Text("document.Paragraph().").LineBreak().LineBreak().
					Text("\tFont(f1).").LineBreak().
					Text("\t\tText(\"This test uses Arial 15, Red. Padding. \", Color.Red).").LineBreak().
					Text("\tFont(f2).").LineBreak().
					Text("\t\tText(\"This text uses Times New Roman Bold 20, Green. Padding. \", Color.Green).").LineBreak().
					Text("\tFont(f3).").LineBreak().
					Text("\t\tText(\"This text uses Courier New Italic 15, Blue. Padding. \", Color.Blue).\n\n").
					Text("\tFont().\n").
					Text("\t\tText(\"Test link (\").\n").
					Text("\t\tLink(\"Test Link 1\", \"Test Link 1\", Color.Blue, new Color(0xFFADD8E6)).\n").
					Text("\t\tText(\"). \").\n\n\n").
					Text("\tFont(f2).\n").
					Text("\t\tText(\"Test link custom font (\", new Color(0xFFFF00FF)).\n").
					Text("\t\tLink(\"Test Link 2\", \"Test Link 2\", Color.Blue, new Color(0xFFADD8E6), f4).\n").
					Text("\t\tText(\").\", new Color(0xFFFF00FF));");

			document.Image("gwen.png", new Size(100, 100));

			label.Document = document;

			return label;
		}

		private void OpenWindow(ControlBase control, EventArgs args)
		{
			Control.Window window = new Control.Window(GetCanvas());
			window.Padding = Padding.Three;
			window.Title = String.Format("RichLabel Window");
			window.DeleteOnClose = true;
			window.Size = new Size(500, 300);
			window.Left = 200; window.Top = 100;

			Control.ScrollControl scroll = new Control.ScrollControl(window);
			scroll.Dock = Dock.Fill;
			scroll.EnableScroll(false, true);
			scroll.AutoHideBars = false;

			Control.RichLabel label = new Control.RichLabel(scroll);
			label.Dock = Dock.Fill;
			label.Document = LongDocument();
		}

		private void OnLinkClicked(ControlBase control, LinkClickedEventArgs args)
		{
			UnitPrint("Link Clicked: " + args.Link);
		}

		private Document LongDocument()
		{
			Document document = new Document();

			document.Paragraph(Margin.Ten, 20).Text
			(
@"In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything."
			);
			document.Image("gwen.png", new Size(60, 60), new Rectangle(32, 32, 448, 448), Color.White, new Margin(20, 10, 20, 10));
			document.Paragraph(Margin.Ten, 20).Text
			(
@"Now it so happened that on one occasion the princess's golden ball did not fall into the little hand which she was holding up for it, but on to the ground beyond, and rolled straight into the water. The king's daughter followed it with her eyes, but it vanished, and the well was deep, so deep that the bottom could not be seen. At this she began to cry, and cried louder and louder, and could not be comforted. And as she thus lamented someone said to her, ""What ails you, king's daughter? You weep so that even a stone would show pity."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"She looked round to the side from whence the voice came, and saw a frog stretching forth its big, ugly head from the water. ""Ah, old water-splasher, is it you,"" she said, ""I am weeping for my golden ball, which has fallen into the well."" ""Be quiet, and do not weep,"" answered the frog, ""I can help you, but what will you give me if I bring your plaything up again?"" ""Whatever you will have, dear frog,"" said she, ""My clothes, my pearls and jewels, and even the golden crown which I am wearing."" The frog answered, ""I do not care for your clothes, your pearls and jewels, nor for your golden crown, but if you will love me and let me be your companion and play-fellow, and sit by you at your little table, and eat off your little golden plate, and drink out of your little cup, and sleep in your little bed - if you will promise me this I will go down below, and bring you your golden ball up again."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"""Oh yes,"" said she, ""I promise you all you wish, if you will but bring me my ball back again."" But she thought, ""How the silly frog does talk. All he does is to sit in the water with the other frogs, and croak. He can be no companion to any human being."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"But the frog when he had received this promise, put his head into the water and sank down; and in a short while came swimmming up again with the ball in his mouth, and threw it on the grass. The king's daughter was delighted to see her pretty plaything once more, and picked it up, and ran away with it. ""Wait, wait,"" said the frog. ""Take me with you. I can't run as you can."" But what did it avail him to scream his croak, croak, after her, as loudly as he could. She did not listen to it, but ran home and soon forgot the poor frog, who was forced to go back into his well again."
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"The next day when she had seated herself at table with the king and all the courtiers, and was eating from her little golden plate, something came creeping splish splash, splish splash, up the marble staircase, and when it had got to the top, it knocked at the door and cried, ""Princess, youngest princess, open the door for me."" She ran to see who was outside, but when she opened the door, there sat the frog in front of it. Then she slammed the door to, in great haste, sat down to dinner again, and was quite frightened. The king saw plainly that her heart was beating violently, and said, ""My child, what are you so afraid of? Is there perchance a giant outside who wants to carry you away?"" ""Ah, no,"" replied she. ""It is no giant but a disgusting frog."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"""What does a frog want with you?"" ""Ah, dear father, yesterday as I was in the forest sitting by the well, playing, my golden ball fell into the water. And because I cried so, the frog brought it out again for me, and because he so insisted, I promised him he should be my companion, but I never thought he would be able to come out of his water. And now he is outside there, and wants to come in to me."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"In the meantime it knocked a second time, and cried, ""Princess, youngest princess, open the door for me, do you not know what you said to me yesterday by the cool waters of the well. Princess, youngest princess, open the door for me."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"Then said the king, ""That which you have promised must you perform. Go and let him in."" She went and opened the door, and the frog hopped in and followed her, step by step, to her chair. There he sat and cried, ""Lift me up beside you."" She delayed, until at last the king commanded her to do it. Once the frog was on the chair he wanted to be on the table, and when he was on the table he said, ""Now, push your little golden plate nearer to me that we may eat together."" She did this, but it was easy to see that she did not do it willingly. The frog enjoyed what he ate, but almost every mouthful she took choked her. At length he said, ""I have eaten and am satisfied, now I am tired, carry me into your little room and make your little silken bed ready, and we will both lie down and go to sleep."""
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"The king's daughter began to cry, for she was afraid of the cold frog which she did not like to touch, and which was now to sleep in her pretty, clean little bed. But the king grew angry and said, ""He who helped you when you were in trouble ought not afterwards to be despised by you."" So she took hold of the frog with two fingers, carried him upstairs, and put him in a corner, but when she was in bed he crept to her and said, ""I am tired, I want to sleep as well as you, lift me up or I will tell your father."" At this she was terribly angry, and took him up and threw him with all her might against the wall. ""Now, will you be quiet, odious frog,"" said she. But when he fell down he was no frog but a king's son with kind and beautiful eyes. He by her father's will was now her dear companion and husband. Then he told her how he had been bewitched by a wicked witch, and how no one could have delivered him from the well but herself, and that to-morrow they would go together into his kingdom."
			);
			document.Paragraph(Margin.Ten, 20).Text
			(
@"Then they went to sleep, and next morning when the sun awoke them, a carriage came driving up with eight white horses, which had white ostrich feathers on their heads, and were harnessed with golden chains, and behind stood the young king's servant Faithful Henry. Faithful Henry had been so unhappy when his master was changed into a frog, that he had caused three iron bands to be laid round his heart, lest it should burst with grief and sadness. The carriage was to conduct the young king into his kingdom. Faithful Henry helped them both in, and placed himself behind again, and was full of joy because of this deliverance. And when they had driven a part of the way the king's son heard a cracking behind him as if something had broken. So he turned round and cried, ""Henry, the carriage is breaking."" ""No, master, it is not the carriage. It is a band from my heart, which was put there in my great pain when you were a frog and imprisoned in the well."" Again and once again while they were on their way something cracked, and each time the king's son thought the carriage was breaking, but it was only the bands which were springing from the heart of Faithful Henry because his master was set free and was happy."
			);

			return document;
		}
	}
}
